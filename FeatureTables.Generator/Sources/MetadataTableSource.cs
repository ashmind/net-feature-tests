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
    public class MetadataTableSource : IFeatureTableSource {
        private readonly IPackageRepository packageRepository;

        public MetadataTableSource(string nugetPackagesPath) {
            this.packageRepository = new SharedPackageRepository(nugetPackagesPath);
        }

        public IEnumerable<FeatureTable> GetTables() {
            var diFrameworks = Frameworks.List().ToArray();
            var packages = new Dictionary<IFrameworkAdapter, IPackage>();
            foreach (var diFramework in diFrameworks) {
                // special case, needed for MEF at least because it is a part of .NET framework:
                if (diFramework.FrameworkPackageId == null)
                    continue;

                var package = packageRepository.FindPackagesById(diFramework.FrameworkPackageId).SingleOrDefault();
                if (package == null)
                    throw new InvalidOperationException("Package for '" + diFramework.FrameworkName + "' was not found in '" + this.packageRepository.Source + "'.");

                packages.Add(diFramework, package);
            }

            yield return GetGeneralInformation(diFrameworks, packages);
            yield return GetNetVersionSupport(diFrameworks, packages);
        }

        private static FeatureTable GetGeneralInformation(IFrameworkAdapter[] diFrameworks, IDictionary<IFrameworkAdapter, IPackage> packages) {
            var version = new Feature(MetadataKeys.VersionFeature, "Version");
            var url = new Feature(MetadataKeys.UrlFeature, "Web Site");
            var total = new Feature(MetadataKeys.TotalFeature, "Total Score") {
                Description = "Total is based on total amount of feature tests passed." + Environment.NewLine + Environment.NewLine +
                              "Most tables give one point per success, but some (such as List support) give one point per table.  " + Environment.NewLine +
                              "The score is only for quick comparison, please read individual tables for the details."
            };

            var table = new FeatureTable(MetadataKeys.GeneralTable, "Framework list", diFrameworks, new[] { version, url, total }) {
                Scoring = FeatureScoring.NotScored
            };
            foreach (var diFramework in diFrameworks) {
                table[diFramework, version].DisplayText = diFramework.FrameworkAssembly.GetName().Version.ToString();
                FillUrl(table[diFramework, url], packages.GetValueOrDefault(diFramework));
            }

            return table;
        }

        private static void FillUrl(FeatureCell cell, IPackage package) {
            if (package == null) { // this is always intentional
                cell.State = FeatureState.Skipped;
                cell.DisplayText = "n/a";
                return;
            }

            if (package.ProjectUrl == null) {
                cell.State = FeatureState.Concern;
                cell.DisplayText = "unknown";
                return;
            }

            cell.State = FeatureState.Neutral;
            cell.DisplayText = "link";
            cell.DisplayUri = package.ProjectUrl;
        }


        private FeatureTable GetNetVersionSupport(IFrameworkAdapter[] diFrameworks, Dictionary<IFrameworkAdapter, IPackage> packages) {
            var allVersionsGrouped = packages.SelectMany(p => p.Value.GetSupportedFrameworks())
                                             .Where(NetFxVersionHelper.ShouldDisplay)
                                             .GroupBy(NetFxVersionHelper.GetDisplayName)
                                             .OrderBy(g => NetFxVersionHelper.GetDisplayOrder(g.First()))
                                             .ToList();

            var versionFeatures = allVersionsGrouped.Select(g => new Feature(g, g.Key));
            var table = new FeatureTable(MetadataKeys.NetFxVersionTable, "Supported .NET versions", diFrameworks, versionFeatures) {
                Description = "This information is based on versions included in NuGet package.",
                Scoring = FeatureScoring.NotScored
            };

            foreach (var diFramework in diFrameworks) {
                var package = packages.GetValueOrDefault(diFramework);
                FillNetVersionSupport(table, diFramework, package, allVersionsGrouped);
            }

            return table;
        }

        private static void FillNetVersionSupport(FeatureTable table, IFrameworkAdapter diFramework, IPackage package, IEnumerable<IGrouping<string, FrameworkName>> allVersionsGrouped) {
            if (package == null) {
                // this is always intentional
                foreach (var versionGroup in allVersionsGrouped) {
                    var cell = table[diFramework, versionGroup];
                    cell.State = FeatureState.Skipped;
                    cell.DisplayText = "n/a";
                }

                return;
            }

            var supported = package.GetSupportedFrameworks().ToArray();

            foreach (var versionGroup in allVersionsGrouped) {
                var cell = table[diFramework, versionGroup];
                if (versionGroup.Any(v => VersionUtility.IsCompatible(v, supported))) {
                    cell.State = FeatureState.Success;
                    cell.DisplayText = "yes";
                }
                else {
                    cell.State = FeatureState.Concern;
                    cell.DisplayText = "no";
                }
            }
        }
    }
}
