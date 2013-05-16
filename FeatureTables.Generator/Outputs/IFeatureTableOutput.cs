using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyInjection.FeatureTables.Generator.Data;

namespace DependencyInjection.FeatureTables.Generator.Outputs {
    public interface IFeatureTableOutput {
        void Write(DirectoryInfo directory, IEnumerable<FeatureTable> tables);
    }
}
