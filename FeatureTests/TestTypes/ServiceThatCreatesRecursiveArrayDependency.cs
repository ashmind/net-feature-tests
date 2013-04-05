using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceThatCreatesRecursiveArrayDependency : IEmptyService {
        public ServiceThatCreatesRecursiveArrayDependency(ServiceWithArrayConstructorDependency dependency) {}
    }
}
