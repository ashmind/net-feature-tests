using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HaveBox;

namespace DependencyInjection.FeatureTests.Adapters {
    public class HaveBoxAdapter : FrameworkAdapterBase {
        private readonly Container container = new Container();
        
        public override Assembly FrameworkAssembly {
            get { return typeof(Container).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.container.Configure(r => r.For(serviceType).Use(implementationType).AsSingleton());
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.container.Configure(r => r.For(serviceType).Use(implementationType));
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.container.Configure(r => r.For(serviceType).Use(() => instance));
        }

        public override object Resolve(Type serviceType) {
            return this.container.GetInstance(serviceType);
        }
    }
}
