using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithSimpleConstructorDependency : IResolvableUnregisteredService {
        private readonly IEmptyService service;

        public ServiceWithSimpleConstructorDependency(IEmptyService service) {
            this.service = service;
        }

        public IEmptyService Service {
            get { return this.service; } 
        }
    }
}
