using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.Adapters {
    public static class FrameworkAdapterExtensions {
        public static void Register<TComponent>(this IFrameworkAdapter container) {
            container.Register<TComponent, TComponent>();
        }

        public static void Register<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.RegisterTransient<TService, TComponent>();
        }

        public static void RegisterSingleton(this IFrameworkAdapter container, Type serviceType, Type componentType) {
            container.RegisterSingleton(serviceType, componentType, null);
        }

        public static void RegisterSingleton<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TComponent));
        }

        public static void RegisterTransient(this IFrameworkAdapter container, Type serviceType, Type componentType) {
            container.RegisterTransient(serviceType, componentType, null);
        }

        public static void RegisterTransient<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TComponent));
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
