using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HaveBox;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class HaveBoxAdapter : ContainerAdapterBase {
        private readonly Container container = new Container();
        
        public override Assembly Assembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Configure(r => r.For(serviceType).Use(implementationType).AsSingleton());
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Configure(r => r.For(serviceType).Use(implementationType));
        }

        public override void RegisterPerWebRequest(Type serviceType, Type implementationType) {
            throw PerWebRequestMayNotBePossible();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.Configure(r => r.For(serviceType).Use(() => instance));
        }

        public override object Resolve(Type serviceType) {
            return this.container.GetInstance(serviceType);
        }
    }
}
