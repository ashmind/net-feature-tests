using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Feature.Tests.Adapters {
    public static class FrameworkAdapterExtensions {
        public static void Add<TComponent>(this IFrameworkAdapter container) {
            container.Add<TComponent, TComponent>();
        }

        public static void Add<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.AddTransient<TService, TComponent>();
        }

        public static void AddSingleton(this IFrameworkAdapter container, Type serviceType, Type componentType) {
            container.RegisterSingleton(serviceType, componentType, null);
        }

        public static void AddSingleton<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.AddSingleton(typeof(TService), typeof(TComponent));
        }

        public static void AddTransient(this IFrameworkAdapter container, Type serviceType, Type componentType) {
            container.RegisterTransient(serviceType, componentType, null);
        }

        public static void AddTransient<TService, TComponent>(this IFrameworkAdapter container) 
            where TComponent : TService
        {
            container.AddTransient(typeof(TService), typeof(TComponent));
        }

        public static void Add<TService>(this IFrameworkAdapter container, TService instance) {
            container.AddInstance(instance);
        }

        public static void AddInstance(this IFrameworkAdapter container, Type serviceType, object instance) {
            container.RegisterInstance(serviceType, instance, null);
        }

        public static void AddInstance<TService>(this IFrameworkAdapter container, TService instance, string key) {
            container.RegisterInstance(typeof(TService), instance, key);
        }

        public static void AddInstance<TService>(this IFrameworkAdapter container, TService instance) {
            container.AddInstance(typeof(TService), instance);
        }

        public static void AddInstance(this IFrameworkAdapter container, object instance) {
            container.AddInstance(instance.GetType(), instance);
        }
    }
}
