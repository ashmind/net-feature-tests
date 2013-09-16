using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    // in real application it should not be static or helper
    public static class DisplayNameHelper {
        public static string GetDisplayName(MemberInfo member) {
            var displayNameAttribute = member.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault();
            if (displayNameAttribute == null)
                return member.Name;

            return displayNameAttribute.DisplayName;
        }
    }
}
