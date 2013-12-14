using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.Adapters.WebRequestSupport;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class CastleAdapter : ContainerAdapterBase {
        private readonly IWindsorContainer container = new WindsorContainer();

        public CastleAdapter() {
            this.container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.container.Kernel));
            this.container.AddFacility<TypedFactoryFacility>();
        }

        public override Assembly Assembly {
            get { return typeof(IWindsorContainer).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, LifestyleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, LifestyleType.Transient);
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, LifestyleType.PerWebRequest);
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

        public override void BeforeAllWebRequests(WebRequestTestHelper helper) {
            helper.RegisterModule<PerWebRequestLifestyleModule>();
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.container.ResolveAll(serviceType).Cast<object>();
        }
    }
}
