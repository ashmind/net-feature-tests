using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FeatureTests.Runner.NuGetGallery;
using FeatureTests.Runner.Sources.MetadataSupport;
using FeatureTests.Shared;
using FeatureTests.Shared.InfrastructureSupport;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Sources {
    public class GeneralInfoTableSource : IFeatureTableSource {
        private readonly MetadataPackageCache packageCache;

        public GeneralInfoTableSource(MetadataPackageCache packageCache) {
            this.packageCache = packageCache;
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
            var downloads = new Feature("Downloads") {
                Description = "Total downloads of the NuGet package."
            };

            var total = new Feature(MetadataKeys.TotalFeature, "Total Score") {
                Description = "Total Score is based on total amount of feature tests passed." + Environment.NewLine + Environment.NewLine +
                              "Most tables give one point per success, but some (such as List support) give one point per table.  " + Environment.NewLine +
                              "The score is only for quick comparison, please read individual tables for the details."
            };

            var libraries = LibraryProvider.GetAdapters(featureTestAssembly).ToArray();
            var table = new FeatureTable(MetadataKeys.GeneralInfoTable, "Mapper list", libraries, new[] { version, released, url, downloads, total }) {
                Scoring = FeatureScoring.NotScored
            };
            
            var tasks = new List<Task>();
            foreach (var library in libraries) {
                table[library, version].DisplayValue = library.Assembly.GetName().Version.ToString();
                this.FillUrl(table[library, url], library);
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
                cell.State = FeatureState.Concern;
                cell.DisplayValue = "unknown";
                return;
            }

            cell.State = FeatureState.Neutral;
            cell.DisplayValue = "link";
            cell.DisplayUri = package.ProjectUrl;
        }

        private async Task FillDataFromNuGetGallery(FeatureTable table, ILibrary library, Feature released, Feature downloads) {
            if (library.PackageId == null) { // this is always intentional
                FillWithNA(table[library, released]);
                FillWithNA(table[library, downloads]);
                return;
            }

            var localPackage = this.packageCache.GetPackage(library.PackageId);
            var localVersion = localPackage.Version.ToString();
            var versionMatchingLocal = await Task.Run(() => {
                var context = new V2FeedContext(new Uri("http://nuget.org/api/v2"));
                return context.Packages.Where(p => p.Id == library.PackageId && p.Version == localVersion).AsEnumerable().Single();
            });

            table[library, released].DisplayValue = new DateTimeOffset(versionMatchingLocal.Published);
            table[library, downloads].DisplayValue = versionMatchingLocal.DownloadCount;
        }

        private static void FillWithNA(FeatureCell cell) {
            cell.State = FeatureState.Skipped;
            cell.DisplayValue = "n/a";
        }
    }
}
