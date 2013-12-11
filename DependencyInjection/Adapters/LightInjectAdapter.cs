using System;
using System.Reflection;
using System.Collections.Generic;
using LightInject;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class LightInjectAdapter : AdapterBase {
        private readonly IServiceContainer container = new ServiceContainer();

        public override Assembly Assembly {
            get { return typeof(IServiceContainer).Assembly; }
        }
        
        public override void RegisterTransient(Type serviceType, Type implementationType) {            
            container.Register(serviceType, implementationType, implementationType.Name);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            container.RegisterInstance(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return container.GetInstance(serviceType);
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            container.Register(serviceType, implementationType, new PerContainerLifetime());
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return container.GetAllInstances(serviceType);
        }
    }
}
