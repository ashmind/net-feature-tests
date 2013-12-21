using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using FeatureTests.Runner.Sources.MetadataSupport;
using FeatureTests.Shared;
using FeatureTests.Shared.InfrastructureSupport;
using FeatureTests.Shared.ResultData;

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
            yield return this.GetTable(featureTestAssembly);
        }

        private FeatureTable GetTable(Assembly featureTestAssembly) {
            var version = new Feature(MetadataKeys.VersionFeature, "Version") {
                Description = "Not necessarily the latest version, but the one we are testing."
            };
            var released = new Feature("Released");
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

            var libraries = LibraryProvider.GetAdapters(featureTestAssembly).ToArray();
            var table = new FeatureTable(MetadataKeys.GeneralInfoTable, @"General information", libraries, new[] {version, released, url, license, downloads, total }) {
                Scoring = FeatureScoring.NotScored
            };
            
            var tasks = new List<Task>();
            foreach (var library in libraries) {
                table[library, version].DisplayValue = library.Assembly.GetName().Version.ToString();
                this.FillUrl(table[library, url], library);
                tasks.Add(this.FillLicense(table[library, license], library));
                tasks.Add(this.FillDataFromNuGetGallery(table, library, released, downloads));
            }

            Task.WaitAll(tasks.ToArray());
            return table;
        }

        private void FillUrl(FeatureCell cell, ILibrary library) {
            if (library.PackageId == null) {
                // this is always intentional
                FillWithNA(cell);
                return;
            }

            var package = this.packageCache.GetPackage(library.PackageId);
            if (package.ProjectUrl == null) {
                FillWithNA(cell);
                return;
            }

            cell.State = FeatureState.Neutral;
            cell.DisplayValue = "link";
            cell.DisplayUri = package.ProjectUrl;
        }
        
        private async Task FillLicense(FeatureCell cell, ILibrary library) {
            if (library.PackageId == null) { // this is always intentional
                FillWithNA(cell);
                return;
            }

            var localPackage = this.packageCache.GetPackage(library.PackageId);
            if (localPackage.LicenseUrl == null) {
                FillWithNA(cell);
                return;
            }

            cell.DisplayUri = localPackage.LicenseUrl;
            var licenseInfo = await this.licenseResolver.GetLicenseInfo(localPackage.LicenseUrl);
            if (licenseInfo == null) {
                cell.State = FeatureState.Neutral;
                cell.DisplayValue = "unrecognized";
                return;
            }

            cell.DisplayValue = licenseInfo.ShortName;
        }

        private async Task FillDataFromNuGetGallery(FeatureTable table, ILibrary library, Feature released, Feature downloads) {
            if (library.PackageId == null) { // this is always intentional
                FillWithNA(table[library, released]);
                FillWithNA(table[library, downloads]);
                return;
            }

            var localPackage = this.packageCache.GetPackage(library.PackageId);
            
            var remoteQueryUrl = new Uri(string.Format("http://nuget.org/api/v2/Packages(Id='{0}',Version='{1}')", localPackage.Id, localPackage.Version));
            var remotePackageXml = XDocument.Parse(await this.dataProvider.GetStringAsync(remoteQueryUrl));

            table[library, released].DisplayValue = GetFromNuGetGalleryResult<DateTimeOffset>(remotePackageXml, "Published");
            table[library, downloads].DisplayValue = GetFromNuGetGalleryResult<int>(remotePackageXml, "DownloadCount");
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
