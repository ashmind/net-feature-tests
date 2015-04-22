using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection.Adapters.Interface {
    public abstract class ContainerAdapterBase : LibraryAdapterBase, IContainerAdapter {
        public abstract void RegisterTransient(Type serviceType, Type implementationType);
        public abstract void RegisterSingleton(Type serviceType, Type implementationType);
        public abstract void RegisterPerWebRequest(Type serviceType, Type implementationType);
        public abstract void RegisterInstance(Type serviceType, object instance);

        public virtual void BeforeAllWebRequests(WebRequestTestHelper helper) { }
        public virtual void AfterBeginWebRequest() {}
        public virtual void BeforeEndWebRequest() {}

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

        protected Exception PerWebRequestMayNotBeProvided() {
            return new NotSupportedException("I am not sure if " + Name + " provides PerRequest lifetime out of the box.");
        }

        protected Exception PerWebRequestMayNotBeProvidedButPerThreadIs() {
            return new NotSupportedException(
                PerWebRequestMayNotBeProvided().Message + Environment.NewLine +
                "It does provide PerThread, but a web request is not guaranteed to stay in one thread."
            );
        }

        protected Exception PerWebRequestMayNotBePossible() {
            return new NotSupportedException("I am not sure if " + Name + " can support PerRequest lifetime.");
        }
    }
}
