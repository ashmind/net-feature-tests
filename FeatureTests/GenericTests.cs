using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests
{
    [DisplayOrder(3)]
    [DisplayName("Generics support")]
    public class GenericTests
    {
        [Feature]
        [DisplayName("Open generic registration")]
        [Description(@"
            Allows registration of open generic types.

            For example Service<> can be registered as IService<>.
            Then any request for IService<T> should be resolved with Service<T>.
        ")]
        public void OpenGenericTypes(IFrameworkAdapter framework)
        {
            framework.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var resolved = framework.Resolve<IGenericService<int>>();

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
            once. Open generic collections can be registerd using RegisterAllOpenGeneric.
            see: http://bit.ly/14DQx7c.
        ", Skip = true)]
        public void ConstrainsForOpenGenerics(IFrameworkAdapter framework)
        {
            framework.RegisterTransient(typeof(IGenericService<>), typeof(GenericService<>));
            framework.RegisterTransient(typeof(IGenericService<>), typeof(GenericServiceWithIService2Constraint<>));
            var resolved = framework.ResolveAll<IGenericService<IndependentService>>().ToArray();

            Assert.Equal(1, resolved.Length);
            Assert.IsType<GenericService<IndependentService>>(resolved[0]);
        }
    }
}
