using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using FeatureTests.Runner.Outputs;
using FeatureTests.Runner.Sources;
using FeatureTests.Runner.Sources.FeatureTestSupport;
using FeatureTests.Runner.Sources.MetadataSupport;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner {
    public static class Program {
        public static void Main(CommandLineArguments args) {
            try {
                var cache = new MetadataPackageCache(Path.GetFullPath(ConfigurationManager.AppSettings["NuGetPackagesPath"]));
                var sources = new IFeatureTableSource[] {
                    new GeneralInfoTableSource(cache),
                    new NetFxSupportTableSource(cache),
                    new FeatureTestTableSource(new FeatureTestRunner())
                };
                var outputs = new IResultOutput[] {
                    new HtmlOutput(new DirectoryInfo(ConfigurationManager.AppSettings["HtmlTemplatesPath"])),
                    new JsonOutput()
                };
                
                var directory = new DirectoryInfo(args.OutputPath ?? ConfigurationManager.AppSettings["OutputPath"]);
                if (!directory.Exists)
                    directory.Create();

                Console.WriteLine("Collecting data...");
                var assembly = Assembly.LoadFrom(args.AssemblyName);
                var tables = sources.SelectMany(s => s.GetTables(assembly)).ToArray();
                CalculateTotal(tables.Single(t => t.Key == MetadataKeys.GeneralInfoTable), tables);

                Console.WriteLine("Creating outputs...");
                var outputNamePrefix = assembly.GetName().Name.SubstringAfter("FeatureTests.On.");
                foreach (var output in outputs) {
                    output.Write(tables, directory, outputNamePrefix, args.WatchTemplates);
                }

                if (args.WatchTemplates) {
                    Console.WriteLine("Auto-updating outputs if templates change.");
                    Console.WriteLine("Press [Enter] to stop...");
                    Console.ReadLine();
                }

                foreach (var output in outputs) {
                    output.Dispose();
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

        public static void Main(string[] args) {
            var parsedArgs = new CommandLineArguments();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, parsedArgs);
            Main(parsedArgs);
        }

        private static void CalculateTotal(FeatureTable general, IReadOnlyCollection<FeatureTable> all) {
            // special case
            var totals = general.Libraries.ToDictionary(
                library => library,
                library => all.Sum(t => t.GetScore(library))
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
