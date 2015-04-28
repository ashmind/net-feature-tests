using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using FeatureTests.Runner.Sources.MetadataSupport;
using FeatureTests.Shared;
using FeatureTests.Shared.InfrastructureSupport;
using FeatureTests.Shared.ResultData;
using NuGet;

namespace FeatureTests.Runner.Sources {
    public class GeneralInfoTableSource : IFeatureTableSource {
        private readonly LocalPackageCache packageCache;
        private readonly HttpDataProvider dataProvider;
        private readonly LicenseResolver licenseResolver;

        public GeneralInfoTableSource(LocalPackageCache packageCache, HttpDataProvider dataProvider, LicenseResolver licenseResolver) {
            this.packageCache = packageCache;
            this.dataProvider = dataProvider;
            this.licenseResolver = licenseResolver;
        }

        public IEnumerable<FeatureTable> GetTables(Assembly featureTestAssembly) {
            yield return GetTable(featureTestAssembly);
        }

        private FeatureTable GetTable(Assembly featureTestAssembly) {
            var version = new Feature(MetadataKeys.VersionFeature, "Version") {
                Description = "Not necessarily the latest version, but the one we are testing."
            };
            var released = new Feature("Released");
            var distribution = new Feature("Distribution");
            var url = new Feature(MetadataKeys.UrlFeature, "Web Site");
            var license = new Feature("License");
            var downloads = new Feature("Downloads") {
                Description = "Total downloads of the NuGet package."
            };
            
            var total = new Feature(MetadataKeys.TotalFeature, "Total Score") {
                Description = "Total Score is based on total amount of feature tests passed." + Environment.NewLine + Environment.NewLine +
                              "Most tables give one point per success, but some (such as List support) give one point per table.  " + Environment.NewLine + Environment.NewLine +
                              "The score is only for quick comparison, please read individual tables for the details."
            };

            var features = new[] { version, released, distribution, url, license, downloads, total };
            var libraries = LibraryProvider.GetAdapters(featureTestAssembly).ToArray();
            var table = new FeatureTable(MetadataKeys.GeneralInfoTable, @"General information", libraries, features) {
                Scoring = FeatureScoring.NotScored
            };
            
            var tasks = new List<Task>();
            foreach (var library in libraries) {
                var package = library.PackageId != null ? this.packageCache.GetPackage(library.PackageId) : null;

                FillVersion(table[library, version], library, package);
                FillDistribution(table[library, distribution], library, package);
                FillUrl(table[library, url], package);
                tasks.Add(FillLicense(table[library, license], package));
                tasks.Add(FillDataFromNuGetGallery(table[library, released], table[library, downloads], package));
            }

            Task.WaitAll(tasks.ToArray());
            return table;
        }

        private static void FillVersion(FeatureCell cell, ILibrary library, IPackage package) {
            cell.DisplayValue = package != null ? package.Version.ToString() : library.Assembly.GetName().Version.ToString();
        }

        private void FillDistribution(FeatureCell cell, ILibrary library, IPackage package) {
            if (package == null) {
                cell.DisplayValue = "built-in";
                return;
            }

            if (library.Assembly == null) {
                cell.State = FeatureState.Concern;
                cell.DisplayValue = "source";
                return;
            }

            cell.DisplayValue = "binary";
        }

        private void FillUrl(FeatureCell cell, IPackage package) {
            if (package == null || package.ProjectUrl == null) {
                FillWithNA(cell);
                return;
            }

            cell.State = FeatureState.Neutral;
            cell.DisplayValue = "link";
            cell.DisplayUri = package.ProjectUrl;
        }

        private async Task FillLicense(FeatureCell cell, IPackage package) {
            if (package == null || package.LicenseUrl == null) {
                FillWithNA(cell);
                return;
            }

            cell.DisplayUri = package.LicenseUrl;
            LicenseInfo licenseInfo = null;
            try {
                licenseInfo = await this.licenseResolver.GetLicenseInfo(package.LicenseUrl);
            }
            catch (HttpDataRequestException ex) {
                cell.State = FeatureState.Neutral;
                cell.DisplayValue = ((int)ex.StatusCode).ToString();
                return;
            }

            if (licenseInfo == null) {
                cell.State = FeatureState.Neutral;
                cell.DisplayValue = "unrecognized";
                return;
            }

            cell.DisplayValue = licenseInfo.ShortName;
        }

        private async Task FillDataFromNuGetGallery(FeatureCell releasedCell, FeatureCell downloadsCell, IPackage localPackage) {
            if (localPackage == null) { // this is always intentional
                FillWithNA(releasedCell);
                FillWithNA(downloadsCell);
                return;
            }
            
            var remoteQueryUrl = new Uri(string.Format("http://nuget.org/api/v2/Packages(Id='{0}',Version='{1}')", localPackage.Id, localPackage.Version));
            var remotePackageXml = XDocument.Parse(await this.dataProvider.GetStringAsync(remoteQueryUrl));

            releasedCell.DisplayValue = GetFromNuGetGalleryResult<DateTimeOffset>(remotePackageXml, "Published");
            downloadsCell.DisplayValue = GetFromNuGetGalleryResult<int>(remotePackageXml, "DownloadCount");
        }

        private T GetFromNuGetGalleryResult<T>(XDocument remotePackageXml, string propertyName) {
            var dataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var element = remotePackageXml.Descendants(XName.Get(propertyName, dataNamespace)).Single();

            return (T)converter.ConvertFromInvariantString(element.Value);
        }

        private static void FillWithNA(FeatureCell cell) {
            cell.State = FeatureState.Skipped;
            cell.DisplayValue = "n/a";
        }
    }
}
