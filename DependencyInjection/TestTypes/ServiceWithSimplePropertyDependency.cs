using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithSimplePropertyDependency {
        [Microsoft.Practices.Unity.Dependency]
        [StructureMap.Attributes.SetterProperty]
        [LinFu.IoC.Configuration.Inject]
        [Ninject.Inject]
        [MugenInjection.Attributes.Inject]
        [IfInjector.Inject]
        [System.ComponentModel.Composition.Import]
        public IService Service { get; set; }
    }
}
