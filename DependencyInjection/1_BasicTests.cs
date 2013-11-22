using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;
using FeatureTests.Shared;
using FeatureTests.On.DependencyInjection.TestTypes;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(1)]
    [DisplayName("Essential functionality")]
    [Description("These are the basic features that should be supported by all frameworks.")]
    public class BasicTests {
        [Feature]
        [DisplayName("Register/resolve independent service")]
        public void IndependentService(IAdapter adapter) {
            adapter.Register<IService, IndependentService>();
            var component = adapter.Resolve<IService>();

            Assert.NotNull(component);
        }

        [Feature]
        [DisplayName("Register/resolve independent service registered as itself")]
        public void IndependentServiceRegisteredAsSelf(IAdapter adapter) {
            adapter.Register<IndependentService>();
            var component = adapter.Resolve<IndependentService>();

            Assert.NotNull(component);
        }

        [Feature]
        [DisplayName("Singleton lifetime")]
        public void SingletonLifetime(IAdapter adapter) {
            adapter.RegisterSingleton<IService, IndependentService>();
            var instance1 = adapter.Resolve<IService>();
            var instance2 = adapter.Resolve<IService>();

            Assert.Same(instance1, instance2);
        }

        [Feature]
        [DisplayName("Transient lifetime")]
        public void TransientLifetime(IAdapter adapter) {
            adapter.RegisterTransient<IService, IndependentService>();
            var instance1 = adapter.Resolve<IService>();
            var instance2 = adapter.Resolve<IService>();

            Assert.NotSame(instance1, instance2);
        }

        [Feature]
        [DisplayName("Register/resolve instance")]
        public void PrebuiltInstance(IAdapter adapter) {
            var instance = new IndependentService();
            adapter.Register<IService>(instance);

            var resolved = adapter.Resolve<IService>();

            Assert.Same(instance, resolved);
        }

        [Feature]
        [DisplayName("Resolve constructor dependency")]
        public void ConstructorDependency(IAdapter adapter) {
            adapter.Register<IService, IndependentService>();
            adapter.Register<ServiceWithSimpleConstructorDependency>();

            var component = adapter.Resolve<ServiceWithSimpleConstructorDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [Feature]
        [DisplayName("Resolve constructor dependency using instance")]
        public void ConstructorDependencyUsingInstance(IAdapter adapter) {
            var instance = new IndependentService();
            adapter.Register<IService>(instance);
            adapter.Register<ServiceWithSimpleConstructorDependency>();

            var dependent = adapter.Resolve<ServiceWithSimpleConstructorDependency>();

            Assert.Same(instance, dependent.Service);
        }
    }
}