using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;

namespace DependencyInjection.FeatureTests.Adapters {
    public class NinjectAdapter : FrameworkAdapterBase {
        private readonly IKernel kernel;

        public NinjectAdapter() {
            this.kernel = new StandardKernel();
        }

        public override Assembly FrameworkAssembly {
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
