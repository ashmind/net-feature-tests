using System;
using System.Collections.Generic;
using System.Linq;

using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace IoC.Framework.Tests.Adapters
{
    public class LinFuAdapter : FrameworkAdapterBase {
        private readonly IServiceContainer _container;

        public LinFuAdapter() {
            _container = new ServiceContainer();
            _container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
        }

        public override void RegisterSingleton(Type serviceType, Type componentType) {
            _container.AddService(serviceType, componentType, LifecycleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            _container.AddService(serviceType, componentType);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            _container.AddService(serviceType, instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (key == null)
                return _container.GetService(serviceType);

            return _container.GetService(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return _container.GetServices(info => serviceType.IsAssignableFrom(info.ServiceType))
                             .Select(info => info.Object);
        }

        public override object CreateInstance(Type componentType) {
            return _container.AutoCreate(componentType);
        }
    }
}
