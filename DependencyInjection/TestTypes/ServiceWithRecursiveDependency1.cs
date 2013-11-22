using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithRecursiveDependency1 {
        public ServiceWithRecursiveDependency1(ServiceWithRecursiveDependency2 dependency) {
        }
    }
}
