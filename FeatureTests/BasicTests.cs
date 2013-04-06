using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    public class BasicTests {
        [ForEachFramework]
        public void IndependentService(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            var component = framework.GetInstance<IService>();

            Assert.NotNull(component);
        }

        [ForEachFramework]
        public void IndependentServiceRegisteredAsSelf(IFrameworkAdapter framework) {
            framework.Register<IndependentService>();
            var component = framework.GetInstance<IndependentService>();

            Assert.NotNull(component);
        }

        [ForEachFramework]
        public void SingletonLifetime(IFrameworkAdapter framework) {
            framework.RegisterSingleton<IService, IndependentService>();
            var instance1 = framework.GetInstance<IService>();
            var instance2 = framework.GetInstance<IService>();

            Assert.Same(instance1, instance2);
        }

        [ForEachFramework]
        public void TransientLifetime(IFrameworkAdapter framework) {
            framework.RegisterTransient<IService, IndependentService>();
            var instance1 = framework.GetInstance<IService>();
            var instance2 = framework.GetInstance<IService>();

            Assert.NotSame(instance1, instance2);
        }

        [ForEachFramework]
        public void PrebuiltInstance(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Register<IService>(instance);

            var resolved = framework.GetInstance<IService>();

            Assert.Same(instance, resolved);
        }

        [ForEachFramework]
        public void ConstructorDependencyUsingInstance(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Register<IService>(instance);
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var dependent = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.Same(instance, dependent.Service);
        }

        [ForEachFramework]
        public void ConstructorDependency(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var component = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }
    }
}