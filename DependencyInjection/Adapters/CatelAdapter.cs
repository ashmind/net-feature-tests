using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catel.IoC;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class CatelAdapter : ContainerAdapterBase {
        private readonly ServiceLocator locator = new ServiceLocator();

        public CatelAdapter() {
            this.locator.RegisterInstance<ITypeFactory>(new TypeFactory(this.locator));
        }
        
        public override Assembly Assembly {
            get { return typeof(ServiceLocator).Assembly; }
        }
        
        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.locator.RegisterType(serviceType, implementationType, registrationType: RegistrationType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.locator.RegisterType(serviceType, implementationType, registrationType: RegistrationType.Transient);
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBePossible();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.locator.RegisterInstance(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.locator.ResolveType(serviceType);
        }
    }
}
