using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithSimpleConstructorDependency : IResolvableUnregisteredService {
        private readonly IService service;

        public ServiceWithSimpleConstructorDependency(IService service) {
            this.service = service;
        }

        public IService Service {
            get { return this.service; } 
        }
    }
}
