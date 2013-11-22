using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using FeatureTests.On.DependencyInjection.Adapters.Interface;

namespace FeatureTests.On.DependencyInjection.Adapters {
    public class NinjectAdapter : AdapterBase {
        private readonly IKernel kernel;

        public NinjectAdapter() {
            this.kernel = new StandardKernel();
        }

        public override Assembly Assembly {
            get { return typeof(IKernel).Assembly; }
        }

        public override void RegisterSingleton(Type serviceType, Type implementationType) {
            this.kernel.Bind(serviceType).To(implementationType).InSingletonScope();
        }

        public override void RegisterTransient(Type serviceType, Type implementationType) {
            this.kernel.Bind(serviceType).To(implementationType).InTransientScope();
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            this.kernel.Bind(serviceType).ToConstant(instance);
        }

        public override object Resolve(Type serviceType) {
            return this.kernel.Get(serviceType);
        }

        public override IEnumerable<object> ResolveAll(Type serviceType) {
            return this.kernel.GetAll(serviceType);
        }
    }
}
