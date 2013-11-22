using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class UnityAdapter : AdapterBase {
        private readonly IUnityContainer container = new UnityContainer();

        public override Assembly Assembly {
            get { return typeof(IUnityContainer).Assembly; }
        }

        public override string PackageId {
            get { return "Unity"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, () => new ContainerControlledLifetimeManager());
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.Register(serviceType, implementationType, () => new TransientLifetimeManager());
        }

        private void Register(Type serviceType, Type implementationType, Func<LifetimeManager> lifetime) {
            this.container.RegisterType(serviceType, implementationType, lifetime());

            var name = serviceType.AssemblyQualifiedName + " => " + implementationType.AssemblyQualifiedName;
            this.container.RegisterType(serviceType, implementationType, name, lifetime());
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.RegisterInstance(serviceType, instance);
            this.container.RegisterInstance(serviceType, serviceType.AssemblyQualifiedName + " => " + instance, instance);
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.container.ResolveAll(serviceType);
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
