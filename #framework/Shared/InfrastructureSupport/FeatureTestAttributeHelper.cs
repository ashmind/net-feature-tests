using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FeatureTests.Shared.InfrastructureSupport {
    // in real application it should not be static or helper
    public static class FeatureTestAttributeHelper {
        public static string GetDisplayName(MemberInfo member) {
            var displayNameAttribute = member.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault();
            if (displayNameAttribute == null)
                return member.Name;

            return displayNameAttribute.DisplayName;
        }

        public static int GetDisplayOrder(MemberInfo member) {
            var displayOrderAttribute = member.GetCustomAttributes<DisplayOrderAttribute>().SingleOrDefault();
            if (displayOrderAttribute == null)
                return int.MaxValue;

            return displayOrderAttribute.Order;
        }

        public static FeatureScoring GetScoring(MemberInfo member) {
            var scoringAttribute = member.GetCustomAttributes<FeatureScoringAttribute>().SingleOrDefault();
            if (scoringAttribute == null)
                return FeatureScoring.PointPerFeature;

            return (FeatureScoring)scoringAttribute.Value;
        }
    }
}
