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
        public const string AssemblyNamePrefix = "FeatureTests.On.";
        // ReSharper disable once InconsistentNaming
        private static readonly FluentConsole.IMainSyntax console = FluentConsole.Instance;

        private static void Main(CommandLineArguments args) {
            var cache = new LocalPackageCache(Path.GetFullPath(ConfigurationManager.AppSettings["NuGetPackagesPath"]));
            var httpDataProvider = new HttpDataProvider(new DirectoryInfo("HttpCache"));
            
            var sources = new IFeatureTableSource[] {
                new GeneralInfoTableSource(
                    cache,
                    httpDataProvider,
                    new LicenseResolver(httpDataProvider, new Uri(ConfigurationManager.AppSettings["LicensesJsonUrl"]))
                ),
                new NetFxSupportTableSource(cache),
                new FeatureTestTableSource(new FeatureTestRunner(), new ExceptionNormalizer())
            };
            var outputs = new IResultOutput[] {
                new HtmlOutput(new DirectoryInfo(ConfigurationManager.AppSettings["HtmlTemplatesPath"])),
                new JsonOutput()
            };
                
            var outputDirectory = new DirectoryInfo(args.OutputPath ?? ConfigurationManager.AppSettings["OutputPath"]);
            if (!outputDirectory.Exists)
                outputDirectory.Create();

            var assemblyPaths = GetAssemblyPaths();
            var results = assemblyPaths.Select(path => {
                console.White.Text("Running " + Path.GetFileName(path) + ":");
                try {
                    var assembly = Assembly.LoadFrom(path);

                    var tables = sources.SelectMany(s => s.GetTables(assembly)).ToArray();
                    CalculateTotal(tables.Single(t => t.Key == MetadataKeys.GeneralInfoTable), tables);

                    var outputNamePrefix = assembly.GetName().Name.SubstringAfter(AssemblyNamePrefix);
                    var result = new ResultForAssembly(assembly, tables, outputNamePrefix);

                    console.Green.Line(" OK");
                    return result;
                }
                catch (Exception) {
                    console.Red.Line(" FAIL");
                    throw;
                }
            }).ToArray();

            console.NewLine();
            console.White.Line("Creating outputs:");
            foreach (var output in outputs) {
                output.Write(outputDirectory, results, args.WatchTemplates);
            }

            if (args.WatchTemplates) {
                console.NewLine()
                       .White
                       .Line("Auto-updating outputs if templates change.")
                       .Line("Press [Enter] to stop.");
                Console.ReadLine();
            }

            foreach (var output in outputs) {
                output.Dispose();
            }

            Console.WriteLine();
            console.Green.Line("Completed.");
        }
        
        public static void Main(string[] args) {
            var parsedArgs = new CommandLineArguments();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, parsedArgs);
            try {
                Main(parsedArgs);
            }
            catch (Exception ex) {
                console.Red.Line(ex);
            }
            Console.ReadKey();
        }

        private static IReadOnlyCollection<string> GetAssemblyPaths() {
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
