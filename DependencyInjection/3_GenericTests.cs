using System.ComponentModel;
using System.Linq;
using Xunit;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.Adapters;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.TestTypes;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(3)]
    [DisplayName("Open generics")]
    public class GenericTests {
        [Feature]
        [DisplayName("Open generic registration")]
        [Description(@"
            Allows registration of open generic types.  
            For example, `Service<>` can be registered as `IService<>`, and then any 
            request for `IService<X>` should be resolved with `Service<X>`.
        ")]
        public void OpenGenericTypes(IContainerAdapter adapter) {
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = adapter.Resolve<IGenericService<int>>();

            Assert.NotNull(resolved);
        }

        [Feature]
        [DependsOnFeature("OpenGenericTypes")]
        [DependsOnFeature(typeof(ListTests), "Enumerable")]
        [DisplayName("Constraint support for open generics")]
        [Description(@"
            For open generic registration, library does not use `Service<T>` to resolve request
            for `IService<X>` if `X` does not match generic constraints on `T`.
        ")]
        public void ConstrainsForOpenGenerics(IContainerAdapter adapter) {
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericServiceWithIService2Constraint<>));
            var resolved = adapter.ResolveAll<IGenericService<IndependentService>>().ToArray();

            Assert.Equal(1, resolved.Length);
            Assert.IsType<GenericService<IndependentService>>(resolved[0]);
        }
    }
}
