using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Outputs {
    public interface IResultOutput : IDisposable {
        void Write(IReadOnlyList<FeatureTable> tables, DirectoryInfo outputDirectory, string outputNamePrefix, bool keepUpdatingIfTemplatesChange = false);
    }
}
