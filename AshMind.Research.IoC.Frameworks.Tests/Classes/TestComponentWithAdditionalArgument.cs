using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public class TestComponentWithAdditionalArgument : TestComponentWithSimpleConstructorDependency {
        private object additionalArgument;
        
        public TestComponentWithAdditionalArgument(ITestService service, object additionalArgument) : base(service) {
            this.additionalArgument = additionalArgument;
        }

        public object AdditionalArgument {
            get { return this.additionalArgument; }
        }
    }
}
