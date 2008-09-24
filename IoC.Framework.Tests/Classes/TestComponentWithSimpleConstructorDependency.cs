using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Tests.Adapters;

namespace IoC.Framework.Tests.Classes {
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
