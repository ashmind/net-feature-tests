using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Tests.Adapters {
    public abstract class FrameworkAdapterBase : ServiceLocatorImplBase, IFrameworkAdapter {
        public abstract void RegisterTransient(Type serviceType, Type componentType);
        public abstract void RegisterSingleton(Type serviceType, Type componentType);
        public abstract void RegisterInstance(Type serviceType, object instance);

        public virtual object CreateInstance(Type componentType) {
            return this.GetInstance(componentType);
        }

        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public IServiceLocator GetLocator() {
            return this;
        }
    }
}
