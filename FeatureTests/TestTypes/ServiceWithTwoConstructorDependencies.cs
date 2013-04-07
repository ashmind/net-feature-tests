using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithTwoConstructorDependencies {
        private readonly IService service1;
        private readonly IService2 service2;

        public ServiceWithTwoConstructorDependencies(IService service1, IService2 service2) {
            this.service1 = service1;
            this.service2 = service2;
        }

        public IService Service1 {
            get { return this.service1; } 
        }

        public IService2 Service2 {
            get { return this.service2; }
        }
    }
}
