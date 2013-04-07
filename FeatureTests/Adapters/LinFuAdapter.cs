using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace DependencyInjection.FeatureTests.Adapters {
    // thanks a lot to Philip Laureano for this adapter
    public class LinFuAdapter : FrameworkAdapterBase {
        private readonly IServiceContainer _container;

        public LinFuAdapter() {
            this._container = new ServiceContainer();
            this._container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.AddService(serviceType, implementationType, LifecycleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.AddService(serviceType, implementationType, LifecycleType.OncePerRequest);
        }

        private void AddService(Type serviceType, Type implementationType, LifecycleType lifecycle) {
            this._container.AddService(serviceType, implementationType, lifecycle);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this._container.AddService(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this._container.GetService(serviceType);
        }
    }
}
