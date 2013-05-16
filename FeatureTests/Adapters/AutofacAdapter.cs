using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace DependencyInjection.FeatureTests.Adapters {
    public class AutofacAdapter : FrameworkAdapterBase {
        private ContainerBuilder builder = new ContainerBuilder();
        private IContainer container;
        
        public override Assembly FrameworkAssembly {
            get { return typeof(IContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                this.builder.RegisterGeneric(implementationType)
                            .As(serviceType)
                            .PropertiesAutowired()
                            .InstancePerLifetimeScope();
            }
            else {
                this.builder.RegisterType(implementationType)
                            .As(serviceType)
                            .PropertiesAutowired()
                            .InstancePerLifetimeScope();
            }
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;
            if (isOpenGeneric) {
                this.builder.RegisterGeneric(implementationType)
                            .As(serviceType)
                            .PropertiesAutowired()
                            .InstancePerDependency();
            }
            else {
                this.builder.RegisterType(implementationType)
                            .As(serviceType)
                            .PropertiesAutowired()
                            .InstancePerDependency();
            }
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.builder.RegisterInstance(instance).As(serviceType);
        }

        public override object Resolve(Type serviceType) {
            this.FreezeContainer();
            return this.container.Resolve(serviceType);
        }

        private void FreezeContainer() {
            if (this.container != null)
                return;

            this.container = this.builder.Build();
            this.builder = null; // simple way to prevent accidental reuse of adapter
        }

        public override bool CrashesOnListRecursion {
            get { return true; }
        }
    }
}
