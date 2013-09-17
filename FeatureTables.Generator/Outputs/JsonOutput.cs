using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Sources;
using DependencyInjection.FeatureTests.Adapters;
using Newtonsoft.Json;
using NuGet;

namespace DependencyInjection.FeatureTables.Generator.Outputs {
    public class JsonOutput : IFeatureTableOutput {
        public void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables) {
            var tableList = tables.ToArray();

            var general = tableList.First(t => t.Key == MetadataKeys.GeneralTable);
            var netFxVersions = tableList.First(t => t.Key == MetadataKeys.NetFxVersionTable);
            var data = tableList[0].Frameworks.Select(f => new {
                name = f.FrameworkName,
                url = general[f, MetadataKeys.UrlFeature].DisplayUri,
                version = general[f, MetadataKeys.VersionFeature].DisplayText,
                supports = GetNetFxVersions(f, netFxVersions),
                features = GetAllFeatureData(f, tableList)
            });

            var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(Path.Combine(directory.FullName, "FeatureData.json"), json);
        }

        private string[] GetNetFxVersions(IFrameworkAdapter framework, FeatureTable table) {
            return table.Features.Where(f => table[framework, f].State == FeatureState.Success)
                                 .Select(f => (IGrouping<string, FrameworkName>)f.Key)
                                 .Select(g => VersionUtility.GetShortFrameworkName(g.First()))
                                 .ToArray();
        }

        private IDictionary<string, object> GetAllFeatureData(IFrameworkAdapter framework, IEnumerable<FeatureTable> tables) {
            return tables.Where(t => t.Key != MetadataKeys.GeneralTable && t.Key != MetadataKeys.NetFxVersionTable)
                         .SelectMany(t => t.Features.Select(f => GetSingleFeatureData(f, t[framework, f])))
                         .ToDictionary(p => p.Key, p => p.Value);
        }

        private KeyValuePair<string, object> GetSingleFeatureData(Feature feature, FeatureCell cell) {
            var key = GetFeatureName(feature);
            var value = new {
                result = cell.State.ToString().ToLowerInvariant(),
                comment = cell.Comment,
                error = cell.RawError
            };
            
            return new KeyValuePair<string, object>(key, value);
        }

        private string GetFeatureName(Feature feature) {
            // HACK :)
            var method = feature.Key as MethodInfo;
            return method != null ? method.Name : feature.Name;
        }
    }
}
