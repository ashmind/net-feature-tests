using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StructureMap;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace IoC.Framework.Feature.Tests.Adapters {
    public class StructureMapAdapter : FrameworkAdapterBase {
        private readonly Registry registry;
        private Container container;

        public StructureMapAdapter() {
            // wow we do not have to use statics
            registry = new Registry();
        }

        public override void AddSingleton(Type serviceType, Type componentType, string key) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIsConcreteType(componentType)
                    .CacheBy(InstanceScope.Singleton);
        }

        public override void AddTransient(Type serviceType, Type componentType, string key) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIsConcreteType(componentType);
        }

        public override void AddInstance(Type serviceType, object instance, string key) {
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
            if (key == null)
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
