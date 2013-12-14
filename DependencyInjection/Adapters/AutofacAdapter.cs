using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class AutofacAdapter : ContainerAdapterBase {
        private ContainerBuilder initialBuilder = new ContainerBuilder();
        private IContainer container;
        private RequestLifetimeScopeProvider requestScopeProvider;

        public override Assembly Assembly {
            get { return typeof(IContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.RegisterType(serviceType, implementationType, r => r.SingleInstance(), r => r.SingleInstance());
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.RegisterType(serviceType, implementationType, r => r.InstancePerDependency(), r => r.InstancePerDependency());
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.RegisterType(serviceType, implementationType, r => r.InstancePerLifetimeScope(), r => r.InstancePerHttpRequest());
        }

        private void RegisterType(
            Type serviceType, Type implementationType,
            Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>> setLifetimeNonGeneric,
            Action<IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>> setLifetimeGeneric
        ) 
        {
            var builder = this.initialBuilder ?? new ContainerBuilder();

            if (!serviceType.IsGenericTypeDefinition) {
                setLifetimeNonGeneric(
                    builder.RegisterType(implementationType)
                           .As(serviceType)
                           .PropertiesAutowired()
                );
            }
            else {
                setLifetimeGeneric(
                    builder.RegisterGeneric(implementationType)
                           .As(serviceType)
                           .PropertiesAutowired()
                );
            }

            if (this.container != null)
                builder.Update(this.container);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            var builder = this.initialBuilder ?? new ContainerBuilder();
            builder.RegisterInstance(instance).As(serviceType);

            if (this.container != null)
                builder.Update(this.container);
        }

        public override void BeforeAllWebRequests(WebRequestSupport.WebRequestTestHelper helper) {
            EnsureContainer();

            // sets the provider on module as well
            // TODO: I am pretty sure there is a race condition (module's provider is in a static field)
            this.requestScopeProvider = new RequestLifetimeScopeProvider(this.container);
            var module = (IHttpModule)Activator.CreateInstance(
                Type.GetType("Autofac.Integration.Mvc.RequestLifetimeHttpModule, Autofac.Integration.Mvc", true)
            );

            helper.RegisterModule(module);
        }
        
        public override object Resolve(Type serviceType) {
            EnsureContainer();
            if (this.requestScopeProvider != null)
                return this.requestScopeProvider.GetLifetimeScope(b => {}).Resolve(serviceType);

            return this.container.Resolve(serviceType);
        }

        private void EnsureContainer() {
            if (this.container != null)
                return;

            this.container = this.initialBuilder.Build();
            this.initialBuilder = null;
        }

        public override bool CrashesOnListRecursion {
            get { return true; }
        }
    }
}
