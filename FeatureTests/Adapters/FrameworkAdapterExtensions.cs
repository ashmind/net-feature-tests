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

        public static void RegisterSingleton(this IFrameworkAdapter container, Type serviceType, Type implementationType) {
            container.RegisterSingleton(serviceType, implementationType, null);
        }

        public static void RegisterSingleton<TService>(this IFrameworkAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterSingleton<TService, TImplementation>(this IFrameworkAdapter container)
            where TImplementation : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TImplementation));
        }

        public static void RegisterTransient(this IFrameworkAdapter container, Type serviceType, Type implementationType) {
            container.RegisterTransient(serviceType, implementationType, null);
        }

        public static void RegisterTransient<TService>(this IFrameworkAdapter container) {
            container.RegisterSingleton<TService, TService>();
        }

        public static void RegisterTransient<TService, TImplementation>(this IFrameworkAdapter container)
            where TImplementation : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TImplementation));
        }

        public static void Register<TService>(this IFrameworkAdapter container, TService instance) {
            container.RegisterInstance(instance);
        }

        public static void RegisterInstance(this IFrameworkAdapter container, Type serviceType, object instance) {
            container.RegisterInstance(serviceType, instance, null);
        }

        public static void RegisterInstance<TService>(this IFrameworkAdapter container, TService instance, string key) {
            container.RegisterInstance(typeof(TService), instance, key);
        }

        public static void RegisterInstance<TService>(this IFrameworkAdapter container, TService instance) {
            container.RegisterInstance(typeof(TService), instance);
        }

        public static void RegisterInstance(this IFrameworkAdapter container, object instance) {
            container.RegisterInstance(instance.GetType(), instance);
        }
    }
}
