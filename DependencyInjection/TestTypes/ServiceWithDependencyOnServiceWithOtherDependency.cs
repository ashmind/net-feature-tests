using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithDependencyOnServiceWithOtherDependency : IResolvableUnregisteredService {
        private readonly ServiceWithSimpleConstructorDependency service;

        public ServiceWithDependencyOnServiceWithOtherDependency(ServiceWithSimpleConstructorDependency service) {
            this.service = service;
        }

        public ServiceWithSimpleConstructorDependency Service {
            get { return this.service; } 
        }
    }
}
