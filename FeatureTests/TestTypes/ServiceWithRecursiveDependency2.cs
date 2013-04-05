using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithRecursiveDependency2 {
        public ServiceWithRecursiveDependency2(ServiceWithRecursiveDependency1 dependency) {
        }
    }
}
