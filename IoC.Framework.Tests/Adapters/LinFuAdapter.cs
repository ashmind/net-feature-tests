using System;
using System.Collections.Generic;
using System.Linq;

using LinFu.IoC;
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
            // how do I make it Singleton in an untyped way?
            _container.AddService(serviceType, componentType);
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            _container.AddService(serviceType, componentType);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            // ashmind: not sure if there is a better way
            Action<object> untyped = _container.AddService;
            var add = untyped.Method.GetGenericMethodDefinition().MakeGenericMethod(serviceType);

            add.Invoke(null, new[] { _container, instance });
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (string.IsNullOrEmpty(key))
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
