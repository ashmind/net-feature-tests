using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.Adapters.Interface {
    public static class ContainerAdapterExtensions {
        public static void RegisterType<TService>(this IContainerAdapter container) {
            container.RegisterTransient<TService, TService>();
        }

        public static void RegisterType<TService, TImplementation>(this IContainerAdapter container)
            where TImplementation : TService 
        {
            container.RegisterTransient<TService, TImplementation>();
        }

        public static void RegisterSingleton<TService>(this IContainerAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterSingleton<TService, TImplementation>(this IContainerAdapter container)
            where TImplementation : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterTransient<TService>(this IContainerAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterTransient<TService, TImplementation>(this IContainerAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TImplementation));
        }

        public static void RegisterPerWebRequest<TService>(this IContainerAdapter container) {
            container.RegisterPerWebRequest<TService, TService>();
        }

        public static void RegisterPerWebRequest<TService, TImplementation>(this IContainerAdapter container)
            where TImplementation : TService 
        {
            container.RegisterPerWebRequest(typeof(TService), typeof(TImplementation));
        }
        
        public static void RegisterInstance<TService>(this IContainerAdapter container, TService instance) {
            container.RegisterInstance(typeof(TService), instance);
        }
        
        public static void RegisterInstance(this IContainerAdapter container, object instance) {
            container.RegisterInstance(instance.GetType(), instance);
        }

        public static TService Resolve<TService>(this IContainerAdapter container) {
            return (TService)container.Resolve(typeof(TService));
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IContainerAdapter container) {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}
