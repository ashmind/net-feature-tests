using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithFuncConstructorDependency {
        private readonly Func<IService> factory;

        public ServiceWithFuncConstructorDependency(Func<IService> factory) {
            this.factory = factory;
        }

        public Func<IService> Factory {
            get { return this.factory; } 
        }
    }
}
