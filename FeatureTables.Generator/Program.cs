using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Outputs;
using DependencyInjection.FeatureTables.Generator.Sources;
using DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport;
using DependencyInjection.FeatureTables.Generator.Sources.MetadataSupport;

namespace DependencyInjection.FeatureTables.Generator {
    public class Program {

        public static void Main(string[] args) {
            try {
                var cache = new MetadataPackageCache(Path.GetFullPath(ConfigurationManager.AppSettings["NuGetPackagesPath"]));
                var sources = new IFeatureTableSource[] {
                    new GeneralInfoTableSource(cache),
                    new NetFxSupportTableSource(cache),
                    new FeatureTestTableSource(new FeatureTestRunner())
                };
                var outputs = new IFeatureTableOutput[] {
                    new HtmlOutput(),
                    new JsonOutput()
                };

                var directory = new DirectoryInfo(args.FirstOrDefault() ?? ConfigurationManager.AppSettings["OutputPath"]);
                if (!directory.Exists)
                    directory.Create();

                Console.WriteLine("Collecting data...");
                var tables = sources.SelectMany(s => s.GetTables()).ToArray();
                CalculateTotal(tables.Single(t => t.Key == MetadataKeys.GeneralInfoTable), tables);

                Console.WriteLine("Creating outputs...");
                foreach (var output in outputs) {
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
                cell.DisplayValue = string.Format("{0:F0}%", percent);
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
