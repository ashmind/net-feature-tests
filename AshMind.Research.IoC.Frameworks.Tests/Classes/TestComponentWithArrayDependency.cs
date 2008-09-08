using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public class TestComponentWithArrayDependency {
        private ITestService[] services;

        public TestComponentWithArrayDependency(ITestService[] services) {
            this.services = services;
        }

        public ITestService[] Services {
            get { return this.services; }
        }
    }
}
