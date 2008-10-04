using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;
using Module=Autofac.Builder.Module;

namespace IoC.Framework.Tests.Adapters {
    public class AutofacAdapter : FrameworkAdapterBase {
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

        public override void RegisterSingleton<TComponent, TService>() {
            builder.Register<TComponent>().As<TService>().SingletonScoped();
        }

        public override void RegisterTransient<TComponent, TService>() {
            builder.Register<TComponent>().As<TService>().FactoryScoped();
        }

        public override void Register<TService>(TService instance) {
            builder.Register(instance).As<TService>();
        }

        public override void RegisterTransient(Type componentType, Type serviceType) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                builder.RegisterGeneric(componentType).As(serviceType);
            }
            else {
                builder.Register(componentType).As(serviceType);
            }
        }

        public override void RegisterAll(Assembly assembly) {
            builder.RegisterTypesFromAssembly(assembly);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            container = container ?? builder.Build();

            // ashmind: will figure this out later
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            container = container ?? builder.Build();

            return string.IsNullOrEmpty(key) ? container.Resolve(serviceType) : container.Resolve(key);
        }
    }
}
