using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;
using Munq;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using Munq.LifetimeManagers;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class MunqAdapter : ContainerAdapterBase {
        private readonly IocContainer container = new IocContainer();
        
        public override Assembly Assembly {
            get { return typeof(IocContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).AsContainerSingleton();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).AsAlwaysNew();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.RegisterInstance(serviceType, instance);
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).AsRequestSingleton();
        }

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
            helper.RegisterModule<RequestLifetimeModule>();
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
