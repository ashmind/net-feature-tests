using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

namespace IoC.Framework.Tests.Adapters {
    public class UnityAdapter : FrameworkAdapterBase {
        private readonly IUnityContainer container = new UnityContainer();

        public override void RegisterSingleton<TComponent, TService>() {
            container.RegisterType<TService, TComponent>(new ContainerControlledLifetimeManager());
        }

        public override void RegisterTransient<TComponent, TService>() {
            container.RegisterType<TService, TComponent>(new TransientLifetimeManager());
        }

        public override void RegisterTransient(Type componentType, Type serviceType) {
            container.RegisterType(serviceType, componentType);
        }

        public override void Register<TService>(TService instance) {
            container.RegisterInstance(instance);
        }

        public override void RegisterAll(Assembly assembly) {
            throw new NotSupportedException();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return container.ResolveAll(serviceType);
        }

        public override TComponent Create<TComponent>() {
            return container.Resolve<TComponent>();
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
