using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

namespace IoC.Framework.Tests.Adapters {
    public class UnityAdapter : FrameworkAdapterBase {
        private readonly IUnityContainer container = new UnityContainer();

        public override void RegisterSingleton(Type serviceType, Type componentType) {
            container.RegisterType(serviceType, componentType, new ContainerControlledLifetimeManager());
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            container.RegisterType(serviceType, componentType, new TransientLifetimeManager());
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            container.RegisterInstance(serviceType, instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return container.ResolveAll(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
