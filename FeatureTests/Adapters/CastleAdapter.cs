using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace DependencyInjection.FeatureTests.Adapters {
    public class CastleAdapter : FrameworkAdapterBase {
        private readonly IKernel kernel = new DefaultKernel();

        public override void RegisterSingleton(Type serviceType, Type implementationType, string key) {
            this.Register(key, serviceType, implementationType, LifestyleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type implementationType, string key) {
            this.Register(key, serviceType, implementationType, LifestyleType.Transient);
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            var registration = Component.For(serviceType).Instance(instance);
            if (key == null)
                registration = registration.Named(key);

            this.kernel.Register(registration);
        }

        private void Register(string key, Type serviceType, Type implementationType, LifestyleType lifestyle) {
            var registration = Component.For(serviceType)
                                        .ImplementedBy(implementationType)
                                        .LifeStyle.Is(lifestyle);

            if (key == null)
                registration = registration.Named(key);

            this.kernel.Register(registration);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (key == null)
                return this.kernel.Resolve(serviceType);

            return this.kernel.Resolve(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return (object[])this.kernel.ResolveAll(serviceType);
        }
    }
}
