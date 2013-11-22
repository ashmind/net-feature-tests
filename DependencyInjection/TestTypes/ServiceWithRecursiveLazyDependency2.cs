using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithRecursiveLazyDependency2 {
        private readonly Lazy<ServiceWithRecursiveLazyDependency1> dependency;

        public ServiceWithRecursiveLazyDependency2(Lazy<ServiceWithRecursiveLazyDependency1> dependency) {
            this.dependency = dependency;
        }

        public ServiceWithRecursiveLazyDependency1 Dependency {
            get { return this.dependency.Value; }
        }
    }
}
