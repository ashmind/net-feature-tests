using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.Documentation {
    // HACK: entry values (not names) must match 
    // DependencyInjection.FeatureTables.Generator.Data.FeatureScoring
    public enum FeatureScoring {
        NotScored,
        PointPerClass,
        PointPerFeature
    }
}
