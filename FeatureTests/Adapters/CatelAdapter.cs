using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catel.IoC;

namespace DependencyInjection.FeatureTests.Adapters {
    public class CatelAdapter : FrameworkAdapterBase {
        private readonly ServiceLocator locator = new ServiceLocator();
        
        public override Assembly FrameworkAssembly {
            get { return typeof(ServiceLocator).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.locator.RegisterType(serviceType, implementationType, registrationType: RegistrationType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.locator.RegisterType(serviceType, implementationType, registrationType: RegistrationType.Transient);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.locator.RegisterInstance(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.locator.ResolveType(serviceType);
        }
    }
}
