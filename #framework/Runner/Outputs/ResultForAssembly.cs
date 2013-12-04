using System.Collections.Generic;
using System.Reflection;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs {
    public class ResultForAssembly {
        public ResultForAssembly(Assembly assembly, IReadOnlyList<FeatureTable> tables, string outputNamePrefix) {
            this.Assembly = assembly;
            this.Tables = tables;
            this.OutputNamePrefix = outputNamePrefix;
        }

        public Assembly Assembly                  { get; private set; }
        public IReadOnlyList<FeatureTable> Tables { get; private set; }
        public string OutputNamePrefix            { get; private set; }
    }
}