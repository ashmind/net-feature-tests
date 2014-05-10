using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class StructureMapAdapter : ContainerAdapterBase {
        private Registry initialRegistry = new Registry();
        private Container container;

        public override Assembly Assembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            RegisterInitialOrConfigure(serviceType, new SingletonLifecycle(), r => r.Use(implementationType));
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            RegisterInitialOrConfigure(serviceType, new TransientLifecycle(), r => r.Use(implementationType));
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBeProvided();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            RegisterInitialOrConfigure(serviceType, new SingletonLifecycle(), r => r.Use(instance));
        }

        private void RegisterInitialOrConfigure(Type serviceType, ILifecycle lifecycle, Action<GenericFamilyExpression> use) {
            var registry = this.initialRegistry ?? new Registry();
            use(registry.For(serviceType).LifecycleIs(lifecycle));

            if (this.container != null)
                this.container.Configure(c => c.AddRegistry(registry));
        }
        
        public override object Resolve(Type serviceType) {
            this.EnsureContainer();
            return this.container.GetInstance(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            this.EnsureContainer();
            return this.container.GetAllInstances(serviceType).Cast<object>();
        }

        private void EnsureContainer() {
            if (this.container != null)
                return;

            this.container = new Container(this.initialRegistry);
            this.initialRegistry = null;
        }
    }
}
