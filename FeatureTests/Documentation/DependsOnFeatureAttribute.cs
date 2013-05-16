using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.Documentation {
    /// <summary>
    /// Defines a feature that must succeed for this test to be run.
    /// This is not currently used by Xunit -- only by FeatureTables.Generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DependsOnFeatureAttribute : Attribute {
        public Type DeclaringType { get; private set; }
        public string MethodName  { get; private set; }

        public DependsOnFeatureAttribute(Type declaringType, string methodName) {
            MethodName = methodName;
            DeclaringType = declaringType;
        }

        // DeclaringType not provided = current type.
        public DependsOnFeatureAttribute(string methodName) {
            MethodName = methodName;
        }
    }
}
