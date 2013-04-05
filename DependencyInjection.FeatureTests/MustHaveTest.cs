using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    public class MustHaveTest {
        [ForEachFramework]
        public void ResolvesJustRegisteredService(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            var component = framework.GetInstance<IEmptyService>();

            Assert.NotNull(component);
        }

        [ForEachFramework]
        public void ResolvesServiceJustRegisteredAsItself(IFrameworkAdapter framework) {
            framework.Add<IndependentService>();
            var component = framework.GetInstance<IndependentService>();

            Assert.NotNull(component);
        }

        [ForEachFramework]
        public void SupportsSingletons(IFrameworkAdapter framework) {
            framework.AddSingleton<IEmptyService, IndependentService>();
            var instance1 = framework.GetInstance<IEmptyService>();
            var instance2 = framework.GetInstance<IEmptyService>();

            Assert.Same(instance1, instance2);
        }

        [ForEachFramework]
        public void SupportsTransients(IFrameworkAdapter framework) {
            framework.AddTransient<IEmptyService, IndependentService>();
            var instance1 = framework.GetInstance<IEmptyService>();
            var instance2 = framework.GetInstance<IEmptyService>();

            Assert.NotSame(instance1, instance2);
        }

        [ForEachFramework]
        public void SupportsInstanceResolution(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Add<IEmptyService>(instance);

            var resolved = framework.GetInstance<IEmptyService>();

            Assert.Same(instance, resolved);
        }

        [ForEachFramework]
        public void SupportsInstanceResolutionForDependency(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Add<IEmptyService>(instance);
            framework.Add<ServiceWithSimpleConstructorDependency>();

            var dependent = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.Same(instance, dependent.Service);
        }

        [ForEachFramework]
        public void SupportsConstructorDependency(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithSimpleConstructorDependency>();

            var component = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [ForEachFramework]
        public void SupportsPropertyDependency(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithSimplePropertyDependency>();

            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }
    }
}