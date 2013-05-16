using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTests;

namespace DependencyInjection.FeatureTables.Generator.Sources {
    public class AssemblyMetadataTableSource : IFeatureTableSource {
        public IEnumerable<FeatureTable> GetTables() {
            var version = new Feature(new object(), "Version");

            var table = new FeatureTable("General information", Frameworks.List(), new[] { version });
            foreach (var framework in table.Frameworks) {
                table[framework, version].Text = framework.FrameworkAssembly.GetName().Version.ToString();
            }

            yield return table;
        }
    }
}
