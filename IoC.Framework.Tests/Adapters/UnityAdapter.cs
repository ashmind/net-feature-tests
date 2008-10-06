using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

namespace IoC.Framework.Tests.Adapters {
    public class UnityAdapter : FrameworkAdapterBase {
        private readonly IUnityContainer container = new UnityContainer();

        public override void AddSingleton(Type serviceType, Type componentType, string key) {
            container.RegisterType(serviceType, componentType, key, new ContainerControlledLifetimeManager());
        }

        public override void AddTransient(Type serviceType, Type componentType, string key) {
            container.RegisterType(serviceType, componentType, key, new TransientLifetimeManager());
        }

        public override void AddInstance(Type serviceType, object instance, string key) {
            container.RegisterInstance(serviceType, key, instance);
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
