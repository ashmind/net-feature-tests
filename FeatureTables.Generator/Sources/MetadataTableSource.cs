using System;
using System.Collections.Generic;
using System.Linq;
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
                var package = this.packageRepository.FindPackagesById(diFramework.FrameworkPackageId).SingleOrDefault();
                if (package == null)
                    throw new InvalidOperationException("Package '" + diFramework.FrameworkPackageId + "' was not found in '" + this.packageRepository.Source + "'.");

                packages.Add(diFramework, package);
            }

            yield return GetGeneralInformation(diFrameworks, packages);
            yield return GetNetVersionSupport(diFrameworks, packages);
        }

        private static FeatureTable GetGeneralInformation(IFrameworkAdapter[] diFrameworks, IDictionary<IFrameworkAdapter, IPackage> packages) {
            var version = new Feature(MetadataKeys.VersionFeature, "Version");
            var url = new Feature(MetadataKeys.UrlFeature, "Web Site");

            var table = new FeatureTable(MetadataKeys.GeneralTable, "General information", diFrameworks, new[] { version, url });
            foreach (var diFramework in diFrameworks) {
                table[diFramework, version].DisplayText = diFramework.FrameworkAssembly.GetName().Version.ToString();
                FillUrl(table[diFramework, url], packages[diFramework]);
            }

            return table;
        }

        private static void FillUrl(FeatureCell cell, IPackage package) {
            if (package.ProjectUrl != null) {
                cell.DisplayText = "link";
                cell.DisplayUri = package.ProjectUrl;
            }
            else {
                cell.State = FeatureState.Concern;
                cell.DisplayText = "unknown";
            }
        }


        private FeatureTable GetNetVersionSupport(IFrameworkAdapter[] diFrameworks, Dictionary<IFrameworkAdapter, IPackage> packages) {
            var allVersions = packages.SelectMany(p => p.Value.GetSupportedFrameworks())
                                      .Distinct()
                                      .Where(NetFxVersionHelper.ShouldDisplay)
                                      .OrderBy(NetFxVersionHelper.GetDisplayOrder)
                                      .ToList();

            var table = new FeatureTable(MetadataKeys.NetFxVersionTable, "Supported .NET versions", diFrameworks, allVersions.Select(v => new Feature(v, NetFxVersionHelper.GetDisplayName(v))));
            table.Description = "This information is based on versions included in NuGet package.";

            foreach (var diFramework in diFrameworks) {
                var supported = packages[diFramework].GetSupportedFrameworks().ToArray();

                foreach (var version in allVersions) {
                    var cell = table[diFramework, version];
                    if (VersionUtility.IsCompatible(version, supported)) {
                        cell.State = FeatureState.Success;
                        cell.DisplayText = "yes";
                    }
                    else {
                        cell.State = FeatureState.Concern;
                        cell.DisplayText = "no";
                    }
                }
            }

            return table;
        }
    }
}
