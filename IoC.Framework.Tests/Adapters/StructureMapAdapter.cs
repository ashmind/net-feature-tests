using System;
using System.Collections.Generic;
using System.Linq;

using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace IoC.Framework.Tests.Adapters {
    public class StructureMapAdapter : IFrameworkAdapter {
        private readonly Registry registry;
        private Container container;

        public StructureMapAdapter() {
            // wow we do not have to use statics
            registry = new Registry();
        }

        public void RegisterSingleton<TComponent, TService>()
            where TComponent : TService
        {
            registry.ForRequestedType<TService>()
                    .TheDefaultIsConcreteType<TComponent>()
                    .AsSingletons();
        }

        public void RegisterTransient<TComponent, TService>()
            where TComponent : TService
        {
            registry.ForRequestedType<TService>()
                    .TheDefaultIsConcreteType<TComponent>();
        }

        public void Register(Type componentType, Type serviceType) {
            registry.ForRequestedType(serviceType)
                    .TheDefaultIsConcreteType(componentType);
        }

        public void Register<TService>(TService instance) {
            registry.ForRequestedType<TService>()
                    .TheDefaultIs(
                        Registry.Object(instance)
                    );
        }

        private void EnsureContainer() {
            if (container != null)
                return;

            container = new Container(this.registry);
        }

        public TService Resolve<TService>() {
            this.EnsureContainer();
            return container.GetInstance<TService>();
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            this.EnsureContainer();
            var additionalArgumentsAsDictionary = additionalArguments.ToDictionary(
                arg => arg.Key, arg => arg.Value
            ); // a pity StructureMap wants a concrete type

            return container.GetInstance<TService>(
                new ExplicitArguments(additionalArgumentsAsDictionary)
            );
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
