using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Feature.Tests.Classes {
    public class TestComponentWithSimplePropertyDependency {
        [Microsoft.Practices.Unity.Dependency]
        [StructureMap.Attributes.SetterProperty]
        [LinFu.IoC.Configuration.Inject]
        public ITestService Service { get; set; }
    }
}
