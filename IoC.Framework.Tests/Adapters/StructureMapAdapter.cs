using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StructureMap;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace IoC.Framework.Tests.Adapters {
    public class StructureMapAdapter : FrameworkAdapterBase {
        private readonly Registry registry;
        private Container container;

        public StructureMapAdapter() {
            // wow we do not have to use statics
            registry = new Registry();
        }

        public override void RegisterSingleton(Type serviceType, Type componentType) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIsConcreteType(componentType)
                    .CacheBy(InstanceScope.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIsConcreteType(componentType);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIs(Registry.Object(instance));
        }

        private void EnsureContainer() {
            if (container != null)
                return;

            container = new Container(this.registry);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            this.EnsureContainer();
            if (string.IsNullOrEmpty(key))
                return container.GetInstance(serviceType);

            return container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            this.EnsureContainer();
            return container.GetAllInstances(serviceType).Cast<object>();
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
