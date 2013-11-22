using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace FeatureTests.Runner {
    public class CommandLineArguments {
        [Option('i', "test-assembly", Required = true, HelpText = "Feature test assembly")]
        public string AssemblyName { get; set; }

        [Option('o', "output-path", HelpText = "Output directory (taken from config if not specified).")]
        public string OutputPath { get; set; }

        [Option('v', "watch-templates", HelpText = "Watches result templates for changes and auto-updates the results.")]
        public bool WatchTemplates { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
