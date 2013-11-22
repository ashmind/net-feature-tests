using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection.Adapters.Interface {
    public abstract class AdapterBase : LibraryAdapterBase, IAdapter {
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
