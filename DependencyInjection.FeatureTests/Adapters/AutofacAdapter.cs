using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Modules;
using Autofac.Registrars;
using DependencyInjection.FeatureTests.TestTypes;

namespace DependencyInjection.FeatureTests.Adapters {
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
            this.builder.RegisterModule(new PropertyInjectionModule());
            this.builder.RegisterModule(new ImplicitCollectionSupportModule());
            this.builder.RegisterTypesAssignableTo<IResolvableUnregisteredService>();
        }

        public override void RegisterSingleton(Type serviceType, Type componentType, string key) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                this.builder.RegisterGeneric(componentType).As(serviceType).SingletonScoped();
            }
            else {
                this.builder.Register(componentType).As(serviceType).SingletonScoped();
            }
        }

        public override void RegisterTransient(Type serviceType, Type componentType, string key) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                this.builder.RegisterGeneric(componentType).As(serviceType).FactoryScoped();
            }
            else {
                this.builder.Register(componentType).As(serviceType).FactoryScoped();
            }
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            // ashmind: not sure if there is a better way
            Func<object, IConcreteRegistrar> method = this.builder.Register;
            var typed = method.Method.GetGenericMethodDefinition().MakeGenericMethod(serviceType);

            var register = ((IConcreteRegistrar)typed.Invoke(null, new[] { this.builder, instance }));
            register.As(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.container = this.container ?? this.builder.Build();

            // ashmind: will figure this out later
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.container = this.container ?? this.builder.Build();

            return string.IsNullOrEmpty(key) ? this.container.Resolve(serviceType) : this.container.Resolve(key);
        }

        public override bool CrashesOnListRecursion {
            get { return true; }
        }
    }
}
