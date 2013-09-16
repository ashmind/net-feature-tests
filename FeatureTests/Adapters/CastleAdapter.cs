using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

namespace DependencyInjection.FeatureTests.Adapters {
    public class CastleAdapter : FrameworkAdapterBase {
        private readonly IWindsorContainer container = new WindsorContainer();

        public CastleAdapter() {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.AddFacility<TypedFactoryFacility>();
        }

        public override Assembly FrameworkAssembly {
            get { return typeof(IKernel).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, LifestyleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, LifestyleType.Transient);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            var registration = Component.For(serviceType).Instance(instance);
            this.container.Register(registration);
        }

        private void Register(Type serviceType, Type implementationType, LifestyleType lifestyle) {
            var registration = Component.For(serviceType)
                                        .ImplementedBy(implementationType)
                                        .LifeStyle.Is(lifestyle);

            this.container.Register(registration);
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.container.ResolveAll(serviceType).Cast<object>();
        }
    }
}
