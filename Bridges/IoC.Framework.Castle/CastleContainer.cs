using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using IoC.Framework.Abstraction;

using Microsoft.Practices.ServiceLocation;

using Castle.Core;
using Castle.MicroKernel;

namespace IoC.Framework.Castle {
    internal class CastleContainer : ServiceLocatorImplBase, IServiceContainer {
        private readonly IKernel kernel = new DefaultKernel();

        public void AddSingleton(Type serviceType, Type componentType, string key) {
            this.Register(key, serviceType, componentType, LifestyleType.Singleton);
        }

        public void AddTransient(Type serviceType, Type componentType, string key) {
            this.Register(key, serviceType, componentType, LifestyleType.Transient);
        }

        public void AddInstance(Type serviceType, object instance, string key) {
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
