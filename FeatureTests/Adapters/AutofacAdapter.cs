using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace DependencyInjection.FeatureTests.Adapters {
    public class AutofacAdapter : FrameworkAdapterBase {
        private readonly ContainerBuilder builder = new ContainerBuilder();
        private IContainer container;
        
        public override void RegisterSingleton(Type serviceType, Type implementationType, string key) {
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

        public override void RegisterTransient(Type serviceType, Type implementationType, string key) {
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

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            this.builder.RegisterInstance(instance).As(serviceType);
        }
        
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.container = this.container ?? this.builder.Build();

            // ashmind: will figure this out later
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.container = this.container ?? this.builder.Build();

            return string.IsNullOrEmpty(key) ? this.container.Resolve(serviceType) : this.container.ResolveNamed(key, serviceType);
        }

        public override bool CrashesOnListRecursion {
            get { return true; }
        }
    }
}
