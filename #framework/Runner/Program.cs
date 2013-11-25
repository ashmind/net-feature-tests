using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AshMind.Extensions;
using FeatureTests.Runner.Outputs;
using FeatureTests.Runner.Sources;
using FeatureTests.Runner.Sources.FeatureTestSupport;
using FeatureTests.Runner.Sources.MetadataSupport;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner {
    public static class Program {
        public static string AssemblyNamePrefix = "FeatureTests.On.";

        private static void Main(CommandLineArguments args) {
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

            var assemblyPaths = GetAssemblyPaths(args);
            var results = assemblyPaths.Select(path => {
                ConsoleEx.WriteLine(ConsoleColor.White, "running " + Path.GetFileName(path));
                var assembly = Assembly.LoadFrom(path);

                var tables = sources.SelectMany(s => s.GetTables(assembly)).ToArray();
                CalculateTotal(tables.Single(t => t.Key == MetadataKeys.GeneralInfoTable), tables);

                var outputNamePrefix = assembly.GetName().Name.SubstringAfter(AssemblyNamePrefix);
                return new ResultOutputArguments(assembly, tables, directory, outputNamePrefix, args.WatchTemplates);
            }).ToArray();

            Console.WriteLine();
            ConsoleEx.WriteLine(ConsoleColor.White, "creating outputs");
            foreach (var result in results) {
                foreach (var output in outputs) {
                    output.Write(result, results);
                }
            }

            if (args.WatchTemplates) {
                Console.WriteLine();
                ConsoleEx.WriteLine(ConsoleColor.White, "auto-updating outputs if templates change.");
                ConsoleEx.WriteLine(ConsoleColor.White, "press [Enter] to stop...");
                Console.WriteLine();
                Console.ReadLine();
            }

            foreach (var output in outputs) {
                output.Dispose();
            }
        }
        
        public static void Main(string[] args) {
            var parsedArgs = new CommandLineArguments();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, parsedArgs);
            try {
                Main(parsedArgs);
            }
            catch (Exception ex) {
                ConsoleEx.WriteLine(ConsoleColor.Red, ex);
                Console.ReadKey();
            }
        }

        private static IReadOnlyCollection<string> GetAssemblyPaths(CommandLineArguments args) {
            return Directory.GetFiles(".", AssemblyNamePrefix + "*.dll");
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
