using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

namespace IoC.Framework.Tests.Adapters {
    public class UnityAdapter : IFrameworkAdapter {
        private readonly IUnityContainer container = new UnityContainer();

        public void RegisterSingleton<TComponent, TService>() 
            where TComponent : TService 
        {
            container.RegisterType<TService, TComponent>(new ContainerControlledLifetimeManager());
        }

        public void RegisterTransient<TComponent, TService>() 
            where TComponent : TService 
        {
            container.RegisterType<TService, TComponent>(new TransientLifetimeManager());
        }

        public void Register(Type componentType, Type serviceType) {
            container.RegisterType(serviceType, componentType);
        }

        public void Register<TService>(TService instance) {
            container.RegisterInstance(instance);
        }

        public void RegisterAll(Assembly assembly) {
            throw new NotSupportedException();
        }

        public TService Resolve<TService>() {
            return container.Resolve<TService>();
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            throw new NotSupportedException();
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
