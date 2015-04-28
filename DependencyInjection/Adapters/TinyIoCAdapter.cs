using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using TinyIoC;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class TinyIoCAdapter : ContainerAdapterBase {
        private readonly TinyIoCContainer container = new TinyIoCContainer();

        public override Assembly Assembly {
            get { return null; }
        }

        public override string PackageId {
            get { return "TinyIoC"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).AsSingleton();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Register(serviceType, implementationType).AsMultiInstance();
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBeProvided();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.Register(serviceType, instance);
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
