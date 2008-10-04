using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel;

namespace IoC.Framework.Tests.Adapters {
    public class CastleAdapter : FrameworkAdapterBase {
        private readonly IKernel kernel = new DefaultKernel();

        public override void RegisterSingleton<TComponent, TService>() {
            this.Register<TComponent, TService>(LifestyleType.Singleton);
        }

        public override void RegisterTransient<TComponent, TService>() {
            this.Register<TComponent, TService>(LifestyleType.Transient);
        } 
        
        private void Register<TComponent, TService>(LifestyleType lifestyle) {
            // a bit awkward, one is generic and one is not
            kernel.AddComponent<TComponent>(typeof(TService), lifestyle);
        }
        
        public override void RegisterTransient(Type componentType, Type serviceType) {
            // why the key is mandatory?
            kernel.AddComponent(serviceType.ToString(), serviceType, componentType);
        }

        public override void Register<TService>(TService instance) {
            // Castle uses generic parameter for key generation only, which is very counterintuitive
            // and I can not skip generic parameter -- instance is object, not TService
            kernel.AddComponentInstance<TService>(typeof(TService), instance);
        }

        public override void RegisterAll(Assembly assembly) {
            // I am cheating a bit here, but using a BatchRegistrationFacility would be major pain,
            // since it is heavily configuration-based
            throw new NotSupportedException();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (string.IsNullOrEmpty(key))
                return kernel.Resolve(serviceType);

            return kernel.Resolve(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            // Major pain 
            Func<object> resolve = kernel.ResolveServices<object>;
            var typed = resolve.Method.GetGenericMethodDefinition().MakeGenericMethod(serviceType);

            return (object[])typed.Invoke(kernel, null);
        }
    }
}
