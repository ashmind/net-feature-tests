using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyInjection.FeatureTests.Documentation {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SpecialCaseAttribute : Attribute {
        public Type FrameworkType { get; private set; }
        public string Comment { get; private set; }

        public SpecialCaseAttribute(Type frameworkType, string comment) {
            FrameworkType = frameworkType;
            Comment = comment;
        }
    }
}
