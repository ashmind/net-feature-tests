using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public override void RegisterSingleton<TComponent, TService>() {
            _container.Inject<TService>().Using<TComponent>().AsSingleton();
        }

        public override void RegisterTransient<TComponent, TService>() {
            _container.Inject<TService>().Using<TComponent>().OncePerRequest();
        }

        public override void Register<TService>(TService instance) {
            _container.AddService(instance);
        }

        public override void RegisterTransient(Type componentType, Type serviceType) {
            _container.AddService(serviceType, componentType);
        }

        public override void RegisterAll(Assembly assembly) {
            _container.LoadFrom(assembly);
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

        public override TComponent Create<TComponent>() {
            return (TComponent) _container.AutoCreate(typeof (TComponent));
        }
    }
}
