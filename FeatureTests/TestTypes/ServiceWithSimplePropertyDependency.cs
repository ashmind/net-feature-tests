using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithSimplePropertyDependency {
        [Microsoft.Practices.Unity.Dependency]
        [StructureMap.Attributes.SetterProperty]
        [LinFu.IoC.Configuration.Inject]
        [Ninject.Inject]
        public IEmptyService Service { get; set; }
    }
}
