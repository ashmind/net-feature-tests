using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Endjin.Core.Container;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class EndjinCompositionAdapter : ContainerAdapterBase {
        private readonly Container container = new Container();

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Register(
                Component.For(serviceType)
                         .ImplementedByType(implementationType)
                         .AsTransient()
            );
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Register(
                Component.For(serviceType)
                         .ImplementedByType(implementationType)
                         .AsSingleton()
            );
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBePossible();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.Register(
                Component.For(serviceType).ImplementedBy(instance)
            );
        }

        public override object Resolve(Type serviceType) {
            return this.container.Resolve(serviceType);
        }

        public override string PackageId {
            get { return "Endjin.Core.Composition"; }
        }

        public override Assembly Assembly {
            get { return typeof(IContainer).Assembly; }
        }

        public override string Name {
            get { return "Endjin Composition"; }
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
