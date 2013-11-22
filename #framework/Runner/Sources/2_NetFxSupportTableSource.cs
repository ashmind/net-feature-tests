using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using FeatureTests.Shared.ResultData;
using NuGet;
using FeatureTests.Shared;
using FeatureTests.Shared.InfrastructureSupport;
using FeatureTests.Runner.Sources.MetadataSupport;

namespace FeatureTests.Runner.Sources {
    public class NetFxSupportTableSource : IFeatureTableSource {
        private readonly MetadataPackageCache packageCache;

        public NetFxSupportTableSource(MetadataPackageCache packageCache) {
            this.packageCache = packageCache;
        }

        public IEnumerable<FeatureTable> GetTables(Assembly featureTestAssembly) {
            yield return this.GetTable(featureTestAssembly);
        }

        private FeatureTable GetTable(Assembly featureTestAssembly) {
            var libraries = LibraryProvider.GetAdapters(featureTestAssembly).ToArray();
            var allVersionsGrouped = libraries.Where(l => l.PackageId != null)
                                              .Select(l => this.packageCache.GetPackage(l.PackageId))
                                              .SelectMany(p => p.GetSupportedFrameworks())
                                              .Where(NetFxVersionHelper.ShouldDisplay)
                                              .GroupBy(NetFxVersionHelper.GetDisplayName)
                                              .OrderBy(g => NetFxVersionHelper.GetDisplayOrder(g.First()))
                                              .ToList();

            var versionFeatures = allVersionsGrouped.Select(g => new Feature(g, g.Key));
            var table = new FeatureTable(MetadataKeys.NetFxSupportTable, "Supported .NET versions", libraries, versionFeatures) {
                Description = "This information is based on versions included in NuGet package.",
                Scoring = FeatureScoring.NotScored
            };

            foreach (var library in libraries) {
                this.FillNetVersionSupport(table, library, allVersionsGrouped);
            }

            return table;
        }

        private void FillNetVersionSupport(FeatureTable table, ILibrary library, IEnumerable<IGrouping<string, FrameworkName>> allVersionsGrouped) {
            if (library.PackageId == null) {
                // this is always intentional
                foreach (var versionGroup in allVersionsGrouped) {
                    var cell = table[library, versionGroup];
                    cell.State = FeatureState.Skipped;
                    cell.DisplayValue = "n/a";
                }

                return;
            }

            var package = this.packageCache.GetPackage(library.PackageId);
            var supported = package.GetSupportedFrameworks().ToArray();

            foreach (var versionGroup in allVersionsGrouped) {
                var cell = table[library, versionGroup];
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
