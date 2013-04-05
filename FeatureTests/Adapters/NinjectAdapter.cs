using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace DependencyInjection.FeatureTests.Adapters {
    public class NinjectAdapter : FrameworkAdapterBase {
        // I do not declare field as IModule, because it does not give access 
        // to Bind.
        //
        // As far as I understand, this is NOT the correct way to do it,
        // you should create your own derived Module. However, for tests
        // this will work.
        //private readonly  module;
        private readonly IKernel kernel;

        public NinjectAdapter() {
            //this.module = new AutoWiringModule();
            this.kernel = new StandardKernel(/*this.module*/);
        }

        public override void RegisterSingleton(Type serviceType, Type componentType, string key) {
            this.kernel.Bind(serviceType).To(componentType).InSingletonScope();
        }

        public override void RegisterTransient(Type serviceType, Type componentType, string key) {
            this.kernel.Bind(serviceType).To(componentType).InTransientScope();
        }

        public override void RegisterInstance(Type serviceType, object instance, string key) {
            this.kernel.Bind(serviceType).ToConstant(instance);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return this.kernel.Get(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            throw new NotImplementedException();
        }
    }
}
