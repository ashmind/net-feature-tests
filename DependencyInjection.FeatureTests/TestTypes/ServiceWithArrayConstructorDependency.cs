using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithArrayConstructorDependency : IServiceWithArrayDependency {
        private readonly IEmptyService[] services;

        public ServiceWithArrayConstructorDependency(IEmptyService[] services) {
            this.services = services;
        }

        public IEmptyService[] Services {
            get { return this.services; }
        }
    }
}
