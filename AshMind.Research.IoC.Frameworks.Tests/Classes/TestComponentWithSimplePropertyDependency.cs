using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public class TestComponentWithSimplePropertyDependency {
        [Microsoft.Practices.Unity.Dependency]
        [StructureMap.Attributes.SetterProperty]
        [LinFu.IoC.Configuration.Inject]
        public ITestService Service { get; set; }
    }
}
