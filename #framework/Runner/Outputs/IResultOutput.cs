using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FeatureTests.Runner.Outputs {
    public interface IResultOutput : IDisposable {
        void Write(DirectoryInfo outputDirectory, IReadOnlyCollection<ResultForAssembly> results, bool keepUpdatingIfTemplatesChange = false);
    }
}
