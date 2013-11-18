using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithRecursiveLazyDependency1 {
        private readonly Lazy<ServiceWithRecursiveLazyDependency2> dependency;

        public ServiceWithRecursiveLazyDependency1(Lazy<ServiceWithRecursiveLazyDependency2> dependency) {
            this.dependency = dependency;
        }

        public ServiceWithRecursiveLazyDependency2 Dependency {
            get { return this.dependency.Value; }
        }
    }
}
