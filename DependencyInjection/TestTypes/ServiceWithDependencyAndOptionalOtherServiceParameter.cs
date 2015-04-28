using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithDependencyAndOptionalOtherServiceParameter {
        public IService2 Optional { get; private set; }

        public ServiceWithDependencyAndOptionalOtherServiceParameter(IService service, IService2 optional = null) {
            Optional = optional;
        }
    }
}
