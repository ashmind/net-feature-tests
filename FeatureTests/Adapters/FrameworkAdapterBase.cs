using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Practices.ServiceLocation;

namespace DependencyInjection.FeatureTests.Adapters {
    public abstract class FrameworkAdapterBase : ServiceLocatorImplBase, IFrameworkAdapter {
        public virtual string FrameworkName {
            get { return Regex.Match(this.GetType().Name, "^(.+?)(?:Adapter)?$").Groups[1].Value; }
        }
        
        public abstract void RegisterTransient(Type serviceType, Type implementationType, string key);
        public abstract void RegisterSingleton(Type serviceType, Type implementationType, string key);
        public abstract void RegisterInstance(Type serviceType, object instance, string key);
        
        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public virtual bool CrashesOnListRecursion {
            get { return this.CrashesOnRecursion; }
        }
    }
}
