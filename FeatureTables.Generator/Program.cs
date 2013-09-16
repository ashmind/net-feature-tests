using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
    }
}
