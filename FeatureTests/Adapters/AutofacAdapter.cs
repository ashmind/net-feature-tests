using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace DependencyInjection.FeatureTests.Adapters {
    public class AutofacAdapter : FrameworkAdapterBase {
        private ContainerBuilder initialBuilder = new ContainerBuilder();
        private IContainer container;
        
        public override Assembly FrameworkAssembly {
            get { return typeof(IContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            var builder = this.initialBuilder ?? new ContainerBuilder();

            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                builder.RegisterGeneric(implementationType)
                       .As(serviceType)
                       .PropertiesAutowired()
                       .InstancePerLifetimeScope();
            }
            else {
                builder.RegisterType(implementationType)
                       .As(serviceType)
                       .PropertiesAutowired()
                       .InstancePerLifetimeScope();
            }

            if (this.container != null)
                builder.Update(this.container);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            var builder = this.initialBuilder ?? new ContainerBuilder();

            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                builder.RegisterGeneric(implementationType)
                       .As(serviceType)
                       .PropertiesAutowired()
                       .InstancePerDependency();
            }
            else {
                builder.RegisterType(implementationType)
                       .As(serviceType)
                       .PropertiesAutowired()
                       .InstancePerDependency();
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

        public override object Resolve(Type serviceType) {
            if (this.container == null) {
                this.container = this.initialBuilder.Build();
                this.initialBuilder = null;
            }

            return this.container.Resolve(serviceType);
        }
        
        public override bool CrashesOnListRecursion {
            get { return true; }
        }
    }
}
