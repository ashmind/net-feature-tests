using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.TestTypes;
using Xunit;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(5)]
    [DisplayName("Lazy<TService>")]
    [FeatureScoring(FeatureScoring.SinglePoint)]
    [Description(@"
        When `TService` is registered in a container, some frameworks automatically provide `Lazy<TService>`.

        This might be useful for expensive initialization or resolution of circular dependencies (you should 
        not have those btw).
    ")]
    public class LazyTests {
        [Feature]
        [DisplayName("Basic support")]
        public void BasicLazySupport(IAdapter adapter) {
            adapter.Register<IService, IndependentService>();
            adapter.Register<ServiceWithSimpleConstructorDependency>();

            var lazy = adapter.Resolve<Lazy<ServiceWithSimpleConstructorDependency>>();

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
        public void NotCreatingLazyPrematurely(IAdapter adapter) {
            adapter.Register<IService, IndependentService>();
            adapter.Register<ServiceWithSimpleConstructorDependency>();

            var lazy = adapter.Resolve<Lazy<ServiceWithSimpleConstructorDependency>>();

            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
        }

        [Feature]
        [DisplayName("Circular dependency")]
        [DependsOnFeature("BasicLazySupport")]
        public void LazyanBeUsedToResolveCircularDepenendency(IAdapter adapter) {
            adapter.RegisterSingleton<ServiceWithRecursiveLazyDependency1>();
            adapter.RegisterSingleton<ServiceWithRecursiveLazyDependency2>();

            var resolved1 = adapter.Resolve<ServiceWithRecursiveLazyDependency1>();
            var resolved2 = adapter.Resolve<ServiceWithRecursiveLazyDependency2>();

            Assert.Same(resolved2, resolved1.Dependency);
            Assert.Same(resolved1, resolved2.Dependency);
        }
    }
}
