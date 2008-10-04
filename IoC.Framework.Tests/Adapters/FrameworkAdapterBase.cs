using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Tests.Adapters {
    public abstract class FrameworkAdapterBase : ServiceLocatorImplBase, IFrameworkAdapter {
        public abstract void RegisterSingleton<TComponent, TService>()
            where TComponent : TService;

        public abstract void RegisterTransient<TComponent, TService>()
            where TComponent : TService;

        public abstract void Register<TService>(TService instance);
        public abstract void RegisterAll(Assembly assembly);
        public abstract void RegisterTransient(Type componentType, Type serviceType);

        public virtual TComponent Create<TComponent>() {
            return this.GetInstance<TComponent>();
        }

        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public IServiceLocator GetLocator() {
            return this;
        }
    }
}
