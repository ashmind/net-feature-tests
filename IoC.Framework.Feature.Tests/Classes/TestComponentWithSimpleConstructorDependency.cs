using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Feature.Tests.Adapters;

namespace IoC.Framework.Feature.Tests.Classes {
    public class TestComponentWithSimpleConstructorDependency : IResolvableUnregisteredService {
        private readonly ITestService service;

        public TestComponentWithSimpleConstructorDependency(ITestService service) {
            this.service = service;
        }

        public ITestService Service {
            get { return this.service; } 
        }
    }
}
