using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace DependencyInjection.FeatureTests.Adapters {
    public class SimpleInjectorAdapter : FrameworkAdapterBase {
        private readonly Container container = new Container();
        
        public override string FrameworkName {
            get { return "Simple Injector"; }
        }

        public override Assembly FrameworkAssembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, Lifestyle.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, Lifestyle.Transient);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.RegisterSingle(serviceType, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.container.GetInstance(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.container.GetAllInstances(serviceType);
        }
        
        private void Register(Type serviceType, Type implementationType, Lifestyle lifestyle) {
            var isOpenGeneric = serviceType.IsGenericTypeDefinition;

            if (isOpenGeneric) {
                this.container.RegisterOpenGeneric(serviceType, implementationType, lifestyle);
            }
            else {
                this.container.Register(serviceType, implementationType, lifestyle);
            }
        }
    }
}