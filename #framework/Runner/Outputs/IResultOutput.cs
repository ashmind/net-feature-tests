using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Runner.Outputs {
    public interface IResultOutput : IDisposable {
        void Write(ResultOutputArguments arguments, IReadOnlyCollection<ResultOutputArguments> allArgumentsForThisRun);
    }
}
