using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            var version = new Feature("Version");
            var url = new Feature("Web Site");

            var table = new FeatureTable("General information", Frameworks.List(), new[] { version, url });
            foreach (var framework in table.Frameworks) {
                var package = this.packageRepository.FindPackagesById(framework.FrameworkPackageId).SingleOrDefault();
                if (package == null)
                    throw new InvalidOperationException("Package '" + framework.FrameworkPackageId + "' was not found in '" + this.packageRepository.Source + "'.");

                table[framework, version].Text = framework.FrameworkAssembly.GetName().Version.ToString();
                FillUrl(table[framework, url], package);
            }

            yield return table;
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
    }
}
