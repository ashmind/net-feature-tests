using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.Adapters {
    public static class FrameworkAdapterExtensions {
        public static void Register<TService>(this IFrameworkAdapter container) {
            container.Register<TService, TService>();
        }

        public static void Register<TService, TImplementation>(this IFrameworkAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient<TService, TImplementation>();
        }

        public static void Register<TService>(this IFrameworkAdapter container, TService instance) {
            container.RegisterInstance(instance);
        }
        
        public static void RegisterSingleton<TService>(this IFrameworkAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterSingleton<TService, TImplementation>(this IFrameworkAdapter container)
            where TImplementation : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterTransient<TService>(this IFrameworkAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterTransient<TService, TImplementation>(this IFrameworkAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterInstance<TService>(this IFrameworkAdapter container, TService instance) {
            container.RegisterInstance(typeof(TService), instance);
        }
        
        public static void RegisterInstance(this IFrameworkAdapter container, object instance) {
            container.RegisterInstance(instance.GetType(), instance);
        }

        public static TService Resolve<TService>(this IFrameworkAdapter container) {
            return (TService)container.Resolve(typeof(TService));
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IFrameworkAdapter container) {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}
