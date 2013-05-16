using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DependencyInjection.FeatureTests.Adapters {
    public abstract class FrameworkAdapterBase : IFrameworkAdapter {
        public abstract Assembly FrameworkAssembly { get; }
        public virtual string FrameworkName {
            get { return Regex.Match(this.GetType().Name, "^(.+?)(?:Adapter)?$").Groups[1].Value; }
        }
        
        public abstract void RegisterTransient(Type serviceType, Type implementationType);
        public abstract void RegisterSingleton(Type serviceType, Type implementationType);
        public abstract void RegisterInstance(Type serviceType, object instance);
        public abstract object Resolve(Type serviceType);

        public virtual IEnumerable<object> ResolveAll(Type serviceType) {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return ((IEnumerable)this.Resolve(enumerableType)).Cast<object>();
        }

        public virtual bool CrashesOnRecursion {
            get { return false; }
        }

        public virtual bool CrashesOnListRecursion {
            get { return this.CrashesOnRecursion; }
        }
    }
}
