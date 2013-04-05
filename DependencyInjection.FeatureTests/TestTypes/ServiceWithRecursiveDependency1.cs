using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithRecursiveDependency1 {
        public ServiceWithRecursiveDependency1(ServiceWithRecursiveDependency2 dependency) {
        }
    }
}
