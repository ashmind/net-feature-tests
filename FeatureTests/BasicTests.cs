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
    [DisplayOrder(1)]
    [DisplayName("Essential functionality")]
    public class BasicTests {
        [DisplayName("Register/resolve independent service")]
        [ForEachFramework]
        public void IndependentService(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            var component = framework.Resolve<IService>();

            Assert.NotNull(component);
        }

        [DisplayName("Register/resolve independent service registered as itself")]
        [ForEachFramework]
        public void IndependentServiceRegisteredAsSelf(IFrameworkAdapter framework) {
            framework.Register<IndependentService>();
            var component = framework.Resolve<IndependentService>();

            Assert.NotNull(component);
        }

        [DisplayName("Singleton lifetime")]
        [ForEachFramework]
        public void SingletonLifetime(IFrameworkAdapter framework) {
            framework.RegisterSingleton<IService, IndependentService>();
            var instance1 = framework.Resolve<IService>();
            var instance2 = framework.Resolve<IService>();

            Assert.Same(instance1, instance2);
        }

        [DisplayName("Transient lifetime")]
        [ForEachFramework]
        public void TransientLifetime(IFrameworkAdapter framework) {
            framework.RegisterTransient<IService, IndependentService>();
            var instance1 = framework.Resolve<IService>();
            var instance2 = framework.Resolve<IService>();

            Assert.NotSame(instance1, instance2);
        }

        [DisplayName("Register/resolve instance")]
        [ForEachFramework]
        public void PrebuiltInstance(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Register<IService>(instance);

            var resolved = framework.Resolve<IService>();

            Assert.Same(instance, resolved);
        }

        [DisplayName("Resolve constructor dependency")]
        [ForEachFramework]
        public void ConstructorDependency(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var component = framework.Resolve<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [DisplayName("Resolve constructor dependency using instance")]
        [ForEachFramework]
        public void ConstructorDependencyUsingInstance(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Register<IService>(instance);
            framework.Register<ServiceWithSimpleConstructorDependency>();

            var dependent = framework.Resolve<ServiceWithSimpleConstructorDependency>();

            Assert.Same(instance, dependent.Service);
        }
    }
}