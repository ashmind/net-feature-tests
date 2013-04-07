using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    [DisplayOrder(3)]
    [DisplayName("Generics support")]
    public class GenericTests {
        [DisplayName("Open generic registration")]
        [Description(@"
            Allows registration of open generic types.

            For example Service<> can be registered as IService<>.
            Then any request for IService<T> should be resolved with Service<T>.
        ")]
        [ForEachFramework]
        public void OpenGenericTypes(IFrameworkAdapter framework) {
            framework.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = framework.GetInstance<IGenericService<int>>();

            Assert.NotNull(resolved);
        }
    }
}
