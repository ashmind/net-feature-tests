using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ninject.Core;
using Ninject.Core.Behavior;
using Ninject.Core.Parameters;
using Ninject.Extensions.AutoWiring;

namespace IoC.Framework.Tests.Adapters {
    public class NinjectAdapter : FrameworkAdapterBase {
        // I do not declare field as IModule, because it does not give access 
        // to Bind.
        //
        // As far as I understand, this is NOT the correct way to do it,
        // you should create your own derived Module. However, for tests
        // this will work.
        private readonly AutoWiringModule module;

        private IKernel kernel;

        public NinjectAdapter() {
            this.module = new AutoWiringModule();
            this.kernel = new StandardKernel(this.module);
        }

        public override void RegisterSingleton<TComponent, TService>() {
            module.Bind<TService>().To<TComponent>().Using<SingletonBehavior>();
        }

        public override void RegisterTransient<TComponent, TService>() {
            module.Bind<TService>().To<TComponent>().Using<TransientBehavior>();
        }

        public override void RegisterTransient(Type componentType, Type serviceType) {
            module.Bind(serviceType).To(componentType);
        }

        public override void Register<TService>(TService instance) {
            module.Bind(typeof(TService)).ToConstant(instance);
        }

        public override void RegisterAll(Assembly assembly) {
            var autoModule = new AutoModule(assembly);
            this.kernel = new StandardKernel(this.module, autoModule);
        }
        
        public override bool CrashesOnRecursion {
            get { return true; }
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            return kernel.Get(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            throw new NotImplementedException();
        }
    }
}
