using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Shared {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class FeatureScoringAttribute : Attribute {
        public FeatureScoring Value { get; private set; }

        public FeatureScoringAttribute(FeatureScoring value) {
            this.Value = value;
        }
    }
}
