using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace AshMind.Research.IoC.Frameworks.Tests.Adapters
{
    public class LinFuAdapter : IFrameworkAdapter
    {
        private readonly IServiceContainer _container;
        public LinFuAdapter()
        {
            _container = new ServiceContainer();
            _container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
        }
        public void RegisterSingleton<TComponent, TService>() where TComponent : TService
        {
            _container.Inject<TService>().Using<TComponent>().AsSingleton();
        }

        public void RegisterTransient<TComponent, TService>() where TComponent : TService
        {
            _container.AddService(typeof(TService), typeof(TComponent));
        }

        public void Register<TService>(TService instance)
        {
            _container.AddService(instance);
        }

        public void Register(Type componentType, Type serviceType)
        {
            _container.AddService(serviceType, componentType);
        }

        public TService Resolve<TService>()
        {
            return (TService) _container.GetService(typeof(TService));
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments)
        {
            var arguments = additionalArguments.Values.ToArray();
            return (TService) _container.AutoCreate(typeof(TService), arguments);
        }

        public TComponent Create<TComponent>()
        {
            return (TComponent) _container.AutoCreate(typeof (TComponent));
        }

        public bool CrashesOnRecursion
        {
            get
            {
                return false;
            }
        }
    }
}
