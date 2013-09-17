using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTables.Generator.Data {
    public enum FeatureState {
        Neutral,
        Success,
        Skipped,
        Concern,
        Failure
    }
}
