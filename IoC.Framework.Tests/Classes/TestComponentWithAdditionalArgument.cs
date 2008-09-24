using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Classes {
    public class TestComponentWithAdditionalArgument : TestComponentWithSimpleConstructorDependency {
        private readonly object additionalArgument;
        
        public TestComponentWithAdditionalArgument(ITestService service, object additionalArgument) : base(service) {
            this.additionalArgument = additionalArgument;
        }

        public object AdditionalArgument {
            get { return this.additionalArgument; }
        }
    }
}
