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
    [DisplayOrder(5)]
    [DisplayName("Support for Lazy<TService>")]
    [FeatureScoring(FeatureScoring.PointPerClass)]
    [Description(@"
        When `TService` is registered in a container, some frameworks automatically provide `Lazy<TService>`.

        This might be useful for expensive initialization or resolution of circular dependencies (you should 
        not have those btw).
    ")]
    public class LazyTests {
        [Feature]
        [DisplayName("Basic support")]
        public void BasicLazySupport(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var lazy = framework.Resolve<Lazy<ServiceWithSimpleConstructorDependency>>();

            Assert.NotNull(lazy);
            Assert.NotNull(lazy.Value);
        }

        [Feature]
        [DisplayName("Not created prematurely")]
        [Description(@"
            The point of `Lazy<T>` is to delay creation of value until `Value` is requested.
            This test verifies that value is not created prematurely.
        ")]
        [DependsOnFeature("BasicLazySupport")]
        public void NotCreatingLazyPrematurely(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var lazy = framework.Resolve<Lazy<ServiceWithSimpleConstructorDependency>>();

            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
        }

        [Feature]
        [DisplayName("Circular dependency")]
        [DependsOnFeature("BasicLazySupport")]
        public void LazyanBeUsedToResolveCircularDepenendency(IFrameworkAdapter framework) {
            framework.RegisterSingleton<ServiceWithRecursiveLazyDependency1>();
            framework.RegisterSingleton<ServiceWithRecursiveLazyDependency2>();

            var resolved1 = framework.Resolve<ServiceWithRecursiveLazyDependency1>();
            var resolved2 = framework.Resolve<ServiceWithRecursiveLazyDependency2>();

            Assert.Same(resolved2, resolved1.Dependency);
            Assert.Same(resolved1, resolved2.Dependency);
        }
    }
}
