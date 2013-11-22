using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.TestTypes;
using FeatureTests.On.DependencyInjection.Adapters;
using Xunit;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(3)]
    [DisplayName("Open generics")]
    public class GenericTests {
        [Feature]
        [DisplayName("Open generic registration")]
        [Description(@"
            Allows registration of open generic types.  
            For example, Service<> can be registered as IService<>, and then any 
            request for IService<X> should be resolved with Service<X>.
        ")]
        public void OpenGenericTypes(IAdapter adapter) {
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = adapter.Resolve<IGenericService<int>>();

            Assert.NotNull(resolved);
        }

        [Feature]
        [DependsOnFeature("OpenGenericTypes")]
        [DependsOnFeature(typeof(ListTests), "Enumerable")]
        [DisplayName("Constraint support for open generics")]
        [Description(@"
            For open generic registration, library does not use Service<T> to resolve request
            for IService<X> if X does not match generic constraints on T.
        ")]
        [SpecialCase(typeof(SimpleInjectorAdapter), @"
            Simple Injector does support filtering based on generic type constraints, but in case of 
            registering a collection of open generic types, it only allows all elements to be registered at 
            once. Open generic collections can be registered using RegisterAllOpenGeneric.
            See: http://bit.ly/14DQx7c.
        ", Skip = true)]
        public void ConstrainsForOpenGenerics(IAdapter adapter) {
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            adapter.RegisterTransient(typeof(IGenericService<>), typeof(GenericServiceWithIService2Constraint<>));
            var resolved = adapter.ResolveAll<IGenericService<IndependentService>>().ToArray();

            Assert.Equal(1, resolved.Length);
            Assert.IsType<GenericService<IndependentService>>(resolved[0]);
        }
    }
}
