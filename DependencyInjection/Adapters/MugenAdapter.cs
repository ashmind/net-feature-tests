using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MugenInjection;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using MugenInjection.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class MugenAdapter : ContainerAdapterBase {
        private readonly MugenInjector injector = new MugenInjector();
        private IInjector webRequestInjector;

        public override Assembly Assembly {
            get { return typeof(MugenInjector).Assembly; }
        }

        public override string PackageId {
            get { return "MugenInjection"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.injector.Bind(serviceType).To(implementationType).InSingletonScope();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.injector.Bind(serviceType).To(implementationType).InTransientScope();
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            this.injector.Bind(serviceType).To(implementationType).InUnitOfWorkScope();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.injector.Bind(serviceType).ToConstant(instance);
        }

        public override void AfterBeginWebRequest() {
            this.webRequestInjector = this.injector.CreateChild();
        }

        public override void BeforeEndWebRequest() {
            this.webRequestInjector.Dispose();
            this.webRequestInjector = null;
        }

        public override object Resolve(Type serviceType) {
            return (this.webRequestInjector ?? this.injector).Get(serviceType);
        }
    }
}
