using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithArrayConstructorDependency : IServiceWithArrayDependency {
        private readonly IService[] services;

        public ServiceWithArrayConstructorDependency(IService[] services) {
            this.services = services;
        }

        public IService[] Services {
            get { return this.services; }
        }
    }
}
