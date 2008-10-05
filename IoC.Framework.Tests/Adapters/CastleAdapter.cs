using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel;

namespace IoC.Framework.Tests.Adapters {
    public class CastleAdapter : FrameworkAdapterBase {
        private readonly IKernel kernel = new DefaultKernel();

        public override void RegisterSingleton(Type serviceType, Type componentType) {
            this.Register(serviceType, componentType, LifestyleType.Singleton);
        }

        public override void RegisterTransient(Type serviceType, Type componentType) {
            this.Register(serviceType, componentType, LifestyleType.Transient);
        }

        public override void RegisterInstance(Type serviceType, object instance) {
            // why the key is mandatory?
            kernel.AddComponentInstance(serviceType.ToString(), serviceType, instance);
        }
        
        private void Register(Type serviceType, Type componentType, LifestyleType lifestyle) {
            // why the key is mandatory?
            kernel.AddComponent(serviceType.ToString(), serviceType, componentType,  lifestyle);
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
