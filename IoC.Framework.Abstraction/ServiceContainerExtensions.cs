using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public static class ServiceContainerExtensions {
        public static void Add<TComponent>(this IServiceContainer container) {
            container.Add<TComponent, TComponent>();
        }

        public static void Add<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.AddTransient<TService, TComponent>();
        }

        public static void AddSingleton(this IServiceContainer container, Type serviceType, Type componentType) {
            container.AddSingleton(serviceType, componentType, null);
        }

        public static void AddSingleton<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.AddSingleton(typeof(TService), typeof(TComponent));
        }

        public static void AddTransient(this IServiceContainer container, Type serviceType, Type componentType) {
            container.AddTransient(serviceType, componentType, null);
        }

        public static void AddTransient<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.AddTransient(typeof(TService), typeof(TComponent));
        }

        public static void Add<TService>(this IServiceContainer container, TService instance) {
            container.AddInstance(instance);
        }

        public static void AddInstance(this IServiceContainer container, Type serviceType, object instance) {
            container.AddInstance(serviceType, instance, null);
        }

        public static void AddInstance<TService>(this IServiceContainer container, TService instance, string key) {
            container.AddInstance(typeof(TService), instance, key);
        }

        public static void AddInstance<TService>(this IServiceContainer container, TService instance) {
            container.AddInstance(typeof(TService), instance);
        }

        public static void AddInstance(this IServiceContainer container, object instance) {
            container.AddInstance(instance.GetType(), instance);
        }
    }
}
