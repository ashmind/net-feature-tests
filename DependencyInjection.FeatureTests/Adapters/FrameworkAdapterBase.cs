using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace DependencyInjection.FeatureTests.Adapters {
    public abstract class FrameworkAdapterBase : ServiceLocatorImplBase, IFrameworkAdapter {
        public abstract void RegisterTransient(Type serviceType, Type componentType, string key);
        public abstract void RegisterSingleton(Type serviceType, Type componentType, string key);
        public abstract void RegisterInstance(Type serviceType, object instance, string key);
        
        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public virtual bool CrashesOnListRecursion {
            get { return this.CrashesOnRecursion; }
        }
    }
}
