using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace DependencyInjection.FeatureTests.Adapters {
    public class StructureMapAdapter : FrameworkAdapterBase {
        private readonly Registry registry;
        private Container container;

        public StructureMapAdapter() {
            this.registry = new Registry();
        }

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

        private void EnsureContainer() {
            if (this.container != null)
                return;

            this.container = new Container(this.registry);
        }
        
        public override object Resolve(Type serviceType) {
            this.EnsureContainer();
            return this.container.GetInstance(serviceType);
        }
    }
}
