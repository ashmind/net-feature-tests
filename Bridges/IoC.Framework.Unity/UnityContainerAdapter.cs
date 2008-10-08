using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using IoC.Framework.Abstraction;

namespace IoC.Framework.Unity {
    internal class UnityContainerAdapter : ServiceLocatorImplBase, IServiceContainer {
        private readonly IUnityContainer container = new UnityContainer();

        public void AddSingleton(Type serviceType, Type componentType, string key) {
            container.RegisterType(serviceType, componentType, key, new ContainerControlledLifetimeManager());
        }

        public void AddTransient(Type serviceType, Type componentType, string key) {
            container.RegisterType(serviceType, componentType, key, new TransientLifetimeManager());
        }

        public void AddInstance(Type serviceType, object instance, string key) {
            container.RegisterInstance(serviceType, key, instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return container.ResolveAll(serviceType);
        }
    }
}
