using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;

namespace DependencyInjection.FeatureTables.Generator.Sources {
    public interface IFeatureTableSource {
        IEnumerable<FeatureTable> GetTables();
    }
}
