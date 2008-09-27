using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinFu.IoC;
using LinFu.IoC.Interfaces;

namespace IoC.Framework.Tests.Adapters
{
    public class LinFuAdapter : IFrameworkAdapter {
        private readonly IServiceContainer _container;

        public LinFuAdapter() {
            _container = new ServiceContainer();
            _container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
        }

        public void RegisterSingleton<TComponent, TService>() 
            where TComponent : TService
        {
            _container.Inject<TService>().Using<TComponent>().AsSingleton();
        }

        public void RegisterTransient<TComponent, TService>() 
            where TComponent : TService 
        {
            // ashmind: I think it is a bit inconsistent with the RegisterSingleton syntax
            _container.AddService(typeof(TService), typeof(TComponent));
        }

        public void Register<TService>(TService instance) {
            _container.AddService(instance);
        }

        public void Register(Type componentType, Type serviceType) {
            _container.AddService(serviceType, componentType);
        }

        public void RegisterAll(Assembly assembly) {
            _container.LoadFrom(assembly);
        }

        public TService Resolve<TService>() {
            return (TService) _container.GetService(typeof(TService));
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            var arguments = additionalArguments.Values.ToArray();
            return (TService) _container.AutoCreate(typeof(TService), arguments);
        }

        public TComponent Create<TComponent>() {
            return (TComponent) _container.AutoCreate(typeof (TComponent));
        }

        public bool CrashesOnRecursion {
            get { return false; }
        }
    }
}
