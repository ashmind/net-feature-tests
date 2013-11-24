using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs {
    public class ResultOutputArguments {
        public ResultOutputArguments(Assembly assembly, IReadOnlyList<FeatureTable> tables, DirectoryInfo outputDirectory, string outputNamePrefix, bool keepUpdatingIfTemplatesChange = false) {
            this.Assembly = assembly;
            this.Tables = tables;
            this.OutputDirectory = outputDirectory;
            this.OutputNamePrefix = outputNamePrefix;
            this.KeepUpdatingIfTemplatesChange = keepUpdatingIfTemplatesChange;
        }

        public Assembly Assembly                  { get; private set; }
        public IReadOnlyList<FeatureTable> Tables { get; private set; }
        public DirectoryInfo OutputDirectory      { get; private set; }
        public string OutputNamePrefix            { get; private set; }
        public bool KeepUpdatingIfTemplatesChange { get; private set; }
    }
}