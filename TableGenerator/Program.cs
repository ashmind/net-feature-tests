using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.TableGenerator.Outputs;
using DependencyInjection.TableGenerator.Sources;

namespace DependencyInjection.TableGenerator {
    public class Program {
        private static readonly IFeatureTableSource[] Sources = { new FeatureTestTableSource() };
        private static readonly IFeatureTableOutput[] Outputs = { new HtmlTableOutput() };

        public static void Main(string[] args) {
            try {
                Console.WriteLine("Collecting data...");
                var tables = Sources.SelectMany(s => s.GetTables()).ToArray();

                Console.WriteLine("Creating outputs...");
                foreach (var output in Outputs) {
                    output.Write(tables);
                }
            }
            catch (Exception ex) {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = color;
            }
        }
    }
}
