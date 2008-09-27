using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel;

namespace IoC.Framework.Tests.Adapters {
    public class CastleAdapter : IFrameworkAdapter {
        private readonly IKernel kernel = new DefaultKernel();

        public void RegisterSingleton<TComponent, TService>()
            where TComponent : TService
        {
            this.Register<TComponent, TService>(LifestyleType.Singleton);
        }

        public void RegisterTransient<TComponent, TService>()
            where TComponent : TService
        {
            this.Register<TComponent, TService>(LifestyleType.Transient);
        } 
        
        private void Register<TComponent, TService>(LifestyleType lifestyle) {
            // a bit awkward, one is generic and one is not
            kernel.AddComponent<TComponent>(typeof(TService), lifestyle);
        }
        
        public void Register(Type componentType, Type serviceType) {
            // why the key is mandatory?
            kernel.AddComponent(serviceType.ToString(), serviceType, componentType);
        }

        public void Register<TService>(TService instance) {
            // Castle uses generic parameter for key generation only, which is very counterintuitive
            // and I can not skip generic parameter -- instance is object, not TService
            kernel.AddComponentInstance<TService>(typeof(TService), instance);
        }

        public void RegisterAll(Assembly assembly) {
            // I am cheating a bit here, but using a BatchRegistrationFacility would be major pain,
            // since it is heavily configuration-based
            throw new NotSupportedException();
        }

        public TService Resolve<TService>() {
            return kernel.Resolve<TService>();
        }

        public TService Resolve<TService>(IDictionary<string, object> additionalArguments) {
            // unfortunately there is no overload for IDictionary<string, object>
            var dictionary = new Dictionary<string, object>(additionalArguments);
            return kernel.Resolve<TService>(dictionary);
        }

        public TComponent Create<TComponent>() {
            return this.Resolve<TComponent>();
        }

        public bool CrashesOnRecursion {
            get { return false; }
        }
    }
}
