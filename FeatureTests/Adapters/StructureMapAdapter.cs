using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace DependencyInjection.FeatureTests.Adapters {
    public class StructureMapAdapter : FrameworkAdapterBase {
        private Registry registry = new Registry();
        private Container container;

        public override Assembly FrameworkAssembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.registry.For(serviceType).Singleton().Use(implementationType);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.registry.For(serviceType).LifecycleIs(InstanceScope.Transient).Use(implementationType);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.registry.For(serviceType).Use(instance);
        }
        
        public override object Resolve(Type serviceType) {
            FreezeContainer();
            return this.container.GetInstance(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            FreezeContainer();
            return this.container.GetAllInstances(serviceType).Cast<object>();
        }

        private void FreezeContainer() {
            if (this.container != null)
                return;

            this.container = new Container(this.registry);
            this.registry = null; // simple way to prevent accidental reuse of adapter
        }
    }
}
