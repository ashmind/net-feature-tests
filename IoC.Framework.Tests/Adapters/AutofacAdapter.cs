using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;
using Module=Autofac.Builder.Module;

namespace IoC.Framework.Tests.Adapters {
    public class AutofacAdapter : IFrameworkAdapter {
        #region PropertyInjectionModule

        public class PropertyInjectionModule : Module {
            protected override void AttachToComponentRegistration(IContainer container, IComponentRegistration registration) {
                registration.Activating += ActivatingHandler.InjectProperties;
            }
        }

        #endregion

        private readonly ContainerBuilder builder = new ContainerBuilder();
        private IContainer container;

        public AutofacAdapter() {
            builder.RegisterModule(new PropertyInjectionModule());
            builder.RegisterTypesAssignableTo<IResolvableUnregisteredService>();
        }

        public void RegisterSingleton<TComponent, TService>() 
            where TComponent : TService 
        {
            builder.Register<TComponent>().As<TService>().SingletonScoped();
        }

        public void RegisterTransient<TComponent, TService>() 
            where TComponent : TService 
        {
            builder.Register<TComponent>().As<TService>().FactoryScoped();
        }

        public void Register<TService>(TService instance) {
            builder.Register(instance).As<TService>();
        }

        public void Register(Type componentType, Type serviceType) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                builder.RegisterGeneric(componentType).As(serviceType);
            }
            else {
                builder.Register(componentType).As(serviceType);
            }
        }

        public void RegisterAll(Assembly assembly) {
            builder.RegisterTypesFromAssembly(assembly);
        }

        public TService Resolve<TService>() {
            return this.Resolve<TService>(new Dictionary<string, object>());
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            var parameters = (
                from pair in additionalArguments
                select new Parameter(pair.Key, pair.Value)
            ).ToArray();

            container = container ?? builder.Build();
            return container.Resolve<TService>(parameters);
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return false; }
        }
    }
}
