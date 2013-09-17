using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DependencyInjection.FeatureTests.Documentation;
using FeatureScoring = DependencyInjection.FeatureTables.Generator.Data.FeatureScoring;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    // in real application it should not be static or helper
    public static class AttributeHelper {
        public static string GetDisplayName(MemberInfo member) {
            var displayNameAttribute = member.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault();
            if (displayNameAttribute == null)
                return member.Name;

            return displayNameAttribute.DisplayName;
        }

        public static FeatureScoring GetScoring(MemberInfo member) {
            var scoringAttribute = member.GetCustomAttributes<FeatureScoringAttribute>().SingleOrDefault();
            if (scoringAttribute == null)
                return FeatureScoring.PointPerFeature;

            return (FeatureScoring)scoringAttribute.Value;
        }
    }
}
