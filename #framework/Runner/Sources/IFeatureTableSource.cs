using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Sources {
    public interface IFeatureTableSource {
        IEnumerable<FeatureTable> GetTables(Assembly featureTestAssembly);
    }
}
