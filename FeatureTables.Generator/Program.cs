using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Outputs;
using DependencyInjection.FeatureTables.Generator.Sources;
using DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport;

namespace DependencyInjection.FeatureTables.Generator {
    public class Program {
        private static readonly IFeatureTableSource[] Sources = {
            new MetadataTableSource(Path.GetFullPath(ConfigurationManager.AppSettings["NuGetPackagesPath"])), 
            new FeatureTestTableSource(new FeatureTestRunner())
        };
        private static readonly IFeatureTableOutput[] Outputs = { new HtmlOutput(), new JsonOutput() };

        public static void Main(string[] args) {
            try {
                var directory = new DirectoryInfo(args.FirstOrDefault() ?? ConfigurationManager.AppSettings["OutputPath"]);
                if (!directory.Exists)
                    directory.Create();

                Console.WriteLine("Collecting data...");
                var tables = Sources.SelectMany(s => s.GetTables()).ToArray();
                CalculateTotal(tables.Single(t => t.Key == MetadataKeys.GeneralTable), tables);

                Console.WriteLine("Creating outputs...");
                foreach (var output in Outputs) {
                    output.Write(directory, tables);
                }
            }
            catch (Exception ex) {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = color;
                Console.ReadKey();
            }
        }

        private static void CalculateTotal(FeatureTable general, IReadOnlyCollection<FeatureTable> all) {
            // special case
            var totals = general.Frameworks.ToDictionary(
                framework => framework,
                framework => all.Sum(t => t.GetScore(framework))
            );

            var max = all.Sum(t => t.MaxScore);
            foreach (var total in totals) {
                var cell = general[total.Key, MetadataKeys.TotalFeature];
                var percent = 100 * ((double)total.Value / max);
                cell.DisplayText = string.Format("{0:F0}%", percent);
                if (percent > 70) {
                    cell.State = FeatureState.Success;
                }
                else if (percent > 40) {
                    cell.State = FeatureState.Concern;
                }
                else {
                    cell.State = FeatureState.Failure;
                }
            }
        }
    }
}
