using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Shared {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class DisplayOrderAttribute : Attribute {
        public int Order { get; private set; }

        public DisplayOrderAttribute(int order) {
            this.Order = order;
        }
    }
}
