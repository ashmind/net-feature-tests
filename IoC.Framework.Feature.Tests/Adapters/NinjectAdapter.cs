using System;
using System.Collections.Generic;
using System.Linq;

using Ninject.Core;
using Ninject.Core.Behavior;
using Ninject.Extensions.AutoWiring;

namespace IoC.Framework.Feature.Tests.Adapters {
    public class NinjectAdapter : FrameworkAdapterBase {
        // I do not declare field as IModule, because it does not give access 
        // to Bind.
        //
        // As far as I understand, this is NOT the correct way to do it,
        // you should create your own derived Module. However, for tests
        // this will work.
        private readonly AutoWiringModule module;
        private readonly IKernel kernel;

        public NinjectAdapter() {
            this.module = new AutoWiringModule();
            this.kernel = new StandardKernel(this.module);
        }

        public override void RegisterSingleton(Type serviceType, Type componentType, string key) {
            module.Bind(serviceType).To(componentType).Using<SingletonBehavior>();
        }

        public override void RegisterTransient(Type serviceType, Type componentType, string key) {
            module.Bind(serviceType).To(componentType).Using<TransientBehavior>();
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            module.Bind(serviceType).ToConstant(instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return kernel.Get(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            throw new NotImplementedException();
        }

        public override bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
