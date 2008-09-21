using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public class TestComponentWithArrayDependency : ITestComponentWithArrayDependency {
        private readonly ITestService[] services;

        public TestComponentWithArrayDependency(ITestService[] services) {
            this.services = services;
        }

        public ITestService[] Services {
            get { return this.services; }
        }
    }
}
