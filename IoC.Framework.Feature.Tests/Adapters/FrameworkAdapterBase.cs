using System;
using System.Collections.Generic;
using System.Linq;
using IoC.Framework.Abstraction;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Feature.Tests.Adapters {
    public abstract class FrameworkAdapterBase : ServiceLocatorImplBase, IFrameworkAdapter {
        public abstract void AddTransient(Type serviceType, Type componentType, string key);
        public abstract void AddSingleton(Type serviceType, Type componentType, string key);
        public abstract void AddInstance(Type serviceType, object instance, string key);

        public virtual object CreateInstance(Type componentType) {
            return this.GetInstance(componentType);
        }

        public IServiceContainer CreateContainer() {
            return this;
        }

        public IServiceLocator CreateLocator(IServiceContainer container) {
            return this;
        }

        public IComponentFactory CreateFactory(IServiceContainer container) {
            return this;
        }

        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public virtual bool CrashesOnListRecursion {
            get { return this.CrashesOnRecursion; }
        }
    }
}
