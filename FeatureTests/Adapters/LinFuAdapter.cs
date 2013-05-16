using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace DependencyInjection.FeatureTests.Adapters {
    // thanks a lot to Philip Laureano for this adapter
    public class LinFuAdapter : FrameworkAdapterBase {
        private readonly IServiceContainer container;

        public LinFuAdapter() {
            this.container = new ServiceContainer();
            this.container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
        }

        public override Assembly FrameworkAssembly {
            get { return typeof(IServiceContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.AddService(serviceType, implementationType, LifecycleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.AddService(serviceType, implementationType, LifecycleType.OncePerRequest);
        }

        private void AddService(Type serviceType, Type implementationType, LifecycleType lifecycle) {
            this.container.AddService(serviceType, implementationType, lifecycle);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.AddService(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.container.GetService(serviceType);
        }
    }
}
