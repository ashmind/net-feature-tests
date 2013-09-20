using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Munq;

namespace DependencyInjection.FeatureTests.Adapters {
    public class MunqAdapter : FrameworkAdapterBase {
        private readonly IocContainer container = new IocContainer();
        
        public override Assembly FrameworkAssembly {
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

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
