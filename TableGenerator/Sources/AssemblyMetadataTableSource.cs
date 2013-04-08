using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Sources {
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
