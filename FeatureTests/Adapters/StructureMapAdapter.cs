using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;

namespace DependencyInjection.FeatureTests.Adapters {
    public class StructureMapAdapter : FrameworkAdapterBase {
        private readonly Registry registry;
        private Container container;

        public StructureMapAdapter() {
            // wow we do not have to use statics
            this.registry = new Registry();
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType, string key) {
            this.registry.For(serviceType).Singleton().Use(implementationType);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType, string key) {
            this.registry.For(serviceType).LifecycleIs(InstanceScope.Transient).Use(implementationType);
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            this.registry.For(serviceType).Use(instance);
        }

        private void EnsureContainer() {
            if (this.container != null)
                return;

            this.container = new Container(this.registry);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.EnsureContainer();
            if (key == null)
                return this.container.GetInstance(serviceType);

            return this.container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.EnsureContainer();
            return this.container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}
