using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AshMind.Extensions;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.NuGetGallery;
using DependencyInjection.FeatureTables.Generator.Sources.MetadataSupport;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using NuGet;

namespace DependencyInjection.FeatureTables.Generator.Sources {
    public class GeneralInfoTableSource : IFeatureTableSource {
        private readonly MetadataPackageCache packageCache;

        public GeneralInfoTableSource(MetadataPackageCache packageCache) {
            this.packageCache = packageCache;
        }

        public IEnumerable<FeatureTable> GetTables() {
            yield return GetTable();
        }

        private FeatureTable GetTable() {
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

            var frameworks = Frameworks.Enumerate().ToArray();
            var table = new FeatureTable(MetadataKeys.GeneralInfoTable, "Framework list", frameworks, new[] { version, released, url, downloads, total }) {
                Scoring = FeatureScoring.NotScored
            };
            
            var tasks = new List<Task>();
            foreach (var framework in frameworks) {
                table[framework, version].DisplayValue = framework.FrameworkAssembly.GetName().Version.ToString();
                FillUrl(table[framework, url], framework);
                tasks.Add(FillDataFromNuGetGallery(table, framework, released, downloads));
            }

            Task.WaitAll(tasks.ToArray());
            return table;
        }

        private void FillUrl(FeatureCell cell, IFrameworkAdapter framework) {
            if (framework.FrameworkPackageId == null) {
                // this is always intentional
                FillWithNA(cell);
                return;
            }

            var package = this.packageCache.GetPackage(framework.FrameworkPackageId);
            if (package.ProjectUrl == null) {
                cell.State = FeatureState.Concern;
                cell.DisplayValue = "unknown";
                return;
            }

            cell.State = FeatureState.Neutral;
            cell.DisplayValue = "link";
            cell.DisplayUri = package.ProjectUrl;
        }

        private async Task FillDataFromNuGetGallery(FeatureTable table, IFrameworkAdapter framework, Feature released, Feature downloads) {
            if (framework.FrameworkPackageId == null) { // this is always intentional
                FillWithNA(table[framework, released]);
                FillWithNA(table[framework, downloads]);
                return;
            }

            var localPackage = this.packageCache.GetPackage(framework.FrameworkPackageId);
            var localVersion = localPackage.Version.ToString();
            var versionMatchingLocal = await Task.Run(() => {
                var context = new V2FeedContext(new Uri("http://nuget.org/api/v2"));
                return context.Packages.Where(p => p.Id == framework.FrameworkPackageId && p.Version == localVersion).AsEnumerable().Single();
            });

            table[framework, released].DisplayValue = new DateTimeOffset(versionMatchingLocal.Published);
            table[framework, downloads].DisplayValue = versionMatchingLocal.DownloadCount;
        }

        private static void FillWithNA(FeatureCell cell) {
            cell.State = FeatureState.Skipped;
            cell.DisplayValue = "n/a";
        }
    }
}
