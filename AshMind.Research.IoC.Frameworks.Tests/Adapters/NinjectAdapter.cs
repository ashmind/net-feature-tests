using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject.Core;
using Ninject.Core.Behavior;
using Ninject.Core.Parameters;
using Ninject.Extensions.AutoWiring;

namespace AshMind.Research.IoC.Frameworks.Tests.Adapters {
    public class NinjectAdapter : IFrameworkAdapter {
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

        public void RegisterSingleton<TComponent, TService>()
            where TComponent : TService
        {
            module.Bind<TService>().To<TComponent>().Using<SingletonBehavior>();
        }

        public void RegisterTransient<TComponent, TService>()
            where TComponent : TService
        {
            module.Bind<TService>().To<TComponent>().Using<TransientBehavior>();
        }

        public void Register(Type componentType, Type serviceType) {
            module.Bind(serviceType).To(componentType);
        }

        public void Register<TService>(TService instance) {
            module.Bind(typeof(TService)).ToConstant(instance);
        }

        public TService Resolve<TService>() {
            return kernel.Get<TService>();
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            // unfortunately there is no overload for IDictionary<string, object>
            var dictionary = new Dictionary<string, object>(additionalArguments);
            return kernel.Get<TService>(
                With.Parameters.ConstructorArguments(dictionary)
            );
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }
        
        public bool CrashesOnRecursion {
            get { return true; }
        }
    }
}
