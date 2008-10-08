using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Test.Classes {
    public class TestComponentRecursingArrayDependency : ITestService {
        public TestComponentRecursingArrayDependency(TestComponentWithArrayDependency dependency) {}
    }
}
