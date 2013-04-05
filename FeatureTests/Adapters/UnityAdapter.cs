using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace DependencyInjection.FeatureTests.Adapters {
    public class UnityAdapter : FrameworkAdapterBase {
        private readonly IUnityContainer container = new UnityContainer();

        public override void RegisterSingleton(Type serviceType, Type componentType, string key) {
            this.container.RegisterType(serviceType, componentType, key, new ContainerControlledLifetimeManager());
        }

        public override void RegisterTransient(Type serviceType, Type componentType, string key) {
            this.container.RegisterType(serviceType, componentType, key, new TransientLifetimeManager());
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            this.container.RegisterInstance(serviceType, key, instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return this.container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return this.container.ResolveAll(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
