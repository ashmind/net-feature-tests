using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using AshMind.Extensions;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Sources.MetadataSupport;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using NuGet;

namespace DependencyInjection.FeatureTables.Generator.Sources {
    public class NetFxSupportTableSource : IFeatureTableSource {
        private readonly MetadataPackageCache packageCache;

        public NetFxSupportTableSource(MetadataPackageCache packageCache) {
            this.packageCache = packageCache;
        }

        public IEnumerable<FeatureTable> GetTables() {
            yield return GetTable();
        }

        private FeatureTable GetTable() {
            var diFrameworks = Frameworks.Enumerate().ToArray();
            var allVersionsGrouped = diFrameworks.Where(f => f.FrameworkPackageId != null)
                                                 .Select(f => this.packageCache.GetPackage(f.FrameworkPackageId))
                                                 .SelectMany(p => p.GetSupportedFrameworks())
                                                 .Where(NetFxVersionHelper.ShouldDisplay)
                                                 .GroupBy(NetFxVersionHelper.GetDisplayName)
                                                 .OrderBy(g => NetFxVersionHelper.GetDisplayOrder(g.First()))
                                                 .ToList();

            var versionFeatures = allVersionsGrouped.Select(g => new Feature(g, g.Key));
            var table = new FeatureTable(MetadataKeys.NetFxSupportTable, "Supported .NET versions", diFrameworks, versionFeatures) {
                Description = "This information is based on versions included in NuGet package.",
                Scoring = FeatureScoring.NotScored
            };

            foreach (var diFramework in diFrameworks) {
                FillNetVersionSupport(table, diFramework,  allVersionsGrouped);
            }

            return table;
        }

        private void FillNetVersionSupport(FeatureTable table, IFrameworkAdapter diFramework, IEnumerable<IGrouping<string, FrameworkName>> allVersionsGrouped) {
            if (diFramework.FrameworkPackageId == null) {
                // this is always intentional
                foreach (var versionGroup in allVersionsGrouped) {
                    var cell = table[diFramework, versionGroup];
                    cell.State = FeatureState.Skipped;
                    cell.DisplayValue = "n/a";
                }

                return;
            }

            var package = this.packageCache.GetPackage(diFramework.FrameworkPackageId);
            var supported = package.GetSupportedFrameworks().ToArray();

            foreach (var versionGroup in allVersionsGrouped) {
                var cell = table[diFramework, versionGroup];
                if (versionGroup.Any(v => VersionUtility.IsCompatible(v, supported))) {
                    cell.State = FeatureState.Success;
                    cell.DisplayValue = "yes";
                }
                else {
                    cell.State = FeatureState.Concern;
                    cell.DisplayValue = "no";
                }
            }
        }
    }
}
