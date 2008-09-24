using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Classes {
    public class TestComponentWithArrayPropertyDependency : ITestComponentWithArrayDependency {
        [Microsoft.Practices.Unity.Dependency]
        [StructureMap.Attributes.SetterProperty]
        [LinFu.IoC.Configuration.Inject]
        public ITestService[] Services { get; set; }
    }
}
