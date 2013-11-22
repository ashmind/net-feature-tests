using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.Adapters.Interface {
    public static class AdapterExtensions {
        public static void Register<TService>(this IAdapter container) {
            container.Register<TService, TService>();
        }

        public static void Register<TService, TImplementation>(this IAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient<TService, TImplementation>();
        }

        public static void Register<TService>(this IAdapter container, TService instance) {
            container.RegisterInstance(instance);
        }
        
        public static void RegisterSingleton<TService>(this IAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterSingleton<TService, TImplementation>(this IAdapter container)
            where TImplementation : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterTransient<TService>(this IAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterTransient<TService, TImplementation>(this IAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterInstance<TService>(this IAdapter container, TService instance) {
            container.RegisterInstance(typeof(TService), instance);
        }
        
        public static void RegisterInstance(this IAdapter container, object instance) {
            container.RegisterInstance(instance.GetType(), instance);
        }

        public static TService Resolve<TService>(this IAdapter container) {
            return (TService)container.Resolve(typeof(TService));
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IAdapter container) {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}
