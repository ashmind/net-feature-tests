using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using DependencyInjection.FeatureTables.Generator.Data;
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
            var frameworks = Frameworks.List().ToArray();
            var packages = new Dictionary<IFrameworkAdapter, IPackage>();
            foreach (var framework in frameworks) {
                var package = this.packageRepository.FindPackagesById(framework.FrameworkPackageId).SingleOrDefault();
                if (package == null)
                    throw new InvalidOperationException("Package '" + framework.FrameworkPackageId + "' was not found in '" + this.packageRepository.Source + "'.");

                packages.Add(framework, package);
            }

            yield return GetGeneralInformation(frameworks, packages);
            yield return GetNetVersionSupport(frameworks, packages);
        }

        private static FeatureTable GetGeneralInformation(IFrameworkAdapter[] frameworks, IDictionary<IFrameworkAdapter, IPackage> packages) {
            var version = new Feature("Version");
            var url = new Feature("Web Site");

            var table = new FeatureTable("General information", frameworks, new[] {version, url});
            foreach (var framework in frameworks) {
                table[framework, version].Text = framework.FrameworkAssembly.GetName().Version.ToString();
                FillUrl(table[framework, url], packages[framework]);
            }

            return table;
        }

        private static void FillUrl(FeatureCell cell, IPackage package) {
            if (package.ProjectUrl != null) {
                cell.Text = "link";
                cell.Uri = package.ProjectUrl;
            }
            else {
                cell.State = FeatureState.Concern;
                cell.Text = "unknown";
            }
        }


        private FeatureTable GetNetVersionSupport(IFrameworkAdapter[] frameworks, Dictionary<IFrameworkAdapter, IPackage> packages) {
            var allVersions = packages.SelectMany(p => p.Value.GetSupportedFrameworks())
                                      .Distinct()
                                      .Where(ShouldDisplayNetVersion)
                                      .OrderBy(GetNetVersionDisplayOrder)
                                      .ToList();

            var table = new FeatureTable("Supported .NET versions", frameworks, allVersions.Select(v => new Feature(v, GetNetVersionName(v))));

            foreach (var framework in frameworks) {
                var supported = packages[framework].GetSupportedFrameworks().ToArray();

                foreach (var version in allVersions) {
                    var cell = table[framework, version];
                    if (VersionUtility.IsCompatible(version, supported)) {
                        cell.State = FeatureState.Success;
                        cell.Text = "yes";
                    }
                    else {
                        cell.State = FeatureState.Concern;
                        cell.Text = "no";
                    }
                }
            }

            return table;
        }

        // TODO: move these 3 methods to a separate class:
        private bool ShouldDisplayNetVersion(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);
            return !normalized.StartsWith("portable");
        }

        private string GetNetVersionDisplayOrder(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);
            var order = "";
            if (normalized.StartsWith("net")) {
                order = "1-";
            }
            else if (normalized.StartsWith("win")) {
                order = "2-";
            }
            else if (normalized.StartsWith("wp")) {
                order = "3-";
            }
            else if (normalized.StartsWith("sl")) {
                order = "4-";
            }

            order += version.Version.Major + "." + version.Version.Minor;
            return order;
        }

        private string GetNetVersionName(FrameworkName version) {
            var normalized = VersionUtility.GetShortFrameworkName(version);

            // HACKS :)
            var result = normalized;
            result = Regex.Replace(result, @"^net(?=\d)", ".NET ");
            result = Regex.Replace(result, @"^(sl\d)\d$", "$1"); // Silverlight normally uses one digit
            result = Regex.Replace(result, @"^sl(?=\d)",  "Silverlight ");
            result = Regex.Replace(result, @"^wp(?=\d)",  "Windows Phone ");
            result = Regex.Replace(result, @"^wp$",       "Windows Phone");
            result = Regex.Replace(result, @"(\d)(\d)",   "$1.$2");
            result = Regex.Replace(result, @"-Client",    " (Client Profile)");

            return result;
        }
    }
}
