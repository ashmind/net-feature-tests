using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithListConstructorDependency<TServiceList> : IServiceWithListDependency<TServiceList> 
        where TServiceList : IEnumerable<IService>
    {
        private readonly TServiceList services;

        public ServiceWithListConstructorDependency(TServiceList services) {
            this.services = services;
        }

        public TServiceList Services {
            get { return this.services; }
        }
    }
}
