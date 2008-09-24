using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Research.IoC.Frameworks.Tests.Adapters;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
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
