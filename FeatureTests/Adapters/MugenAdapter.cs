using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MugenInjection;

namespace DependencyInjection.FeatureTests.Adapters {
    public class MugenAdapter : FrameworkAdapterBase {
        private readonly MugenInjector injector = new MugenInjector();
        
        public override Assembly FrameworkAssembly {
            get { return typeof(MugenInjector).Assembly; }
        }

        public override string FrameworkPackageId {
            get { return "MugenInjection"; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.injector.Bind(serviceType).To(implementationType).InSingletonScope();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.injector.Bind(serviceType).To(implementationType).InTransientScope();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.injector.Bind(serviceType).ToConstant(instance);
        }

        public override object Resolve(Type serviceType) {
            return this.injector.Get(serviceType);
        }
    }
}
