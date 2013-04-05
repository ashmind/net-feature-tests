using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace IoC.Framework.Feature.Tests.Adapters {
    public class CastleAdapter : FrameworkAdapterBase {
        private readonly IKernel kernel = new DefaultKernel();

        public override void AddSingleton(Type serviceType, Type componentType, string key) {
            this.Register(key, serviceType, componentType, LifestyleType.Singleton);
        }

        public override void AddTransient(Type serviceType, Type componentType, string key) {
            this.Register(key, serviceType, componentType, LifestyleType.Transient);
        }

        public override void AddInstance(Type serviceType, object instance, string key) {
            var registration = Component.For(serviceType).Instance(instance);
            if (key == null)
                registration = registration.Named(key);

            kernel.Register(registration);
        }

        private void Register(string key, Type serviceType, Type componentType, LifestyleType lifestyle) {
            var registration = Component.For(serviceType)
                                        .ImplementedBy(componentType)
                                        .LifeStyle.Is(lifestyle);

            if (key == null)
                registration = registration.Named(key);

            kernel.Register(registration);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (key == null)
                return kernel.Resolve(serviceType);

            return kernel.Resolve(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return (object[])kernel.ResolveAll(serviceType);
        }
    }
}
