using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public static class ServiceContainerExtensions {
        public static void Register<TComponent>(this IServiceContainer container) {
            container.Register<TComponent, TComponent>();
        }

        public static void Register<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.RegisterTransient<TService, TComponent>();
        }

        public static void RegisterSingleton<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.RegisterSingleton(typeof(TService), typeof(TComponent));
        }

        public static void RegisterTransient<TService, TComponent>(this IServiceContainer container) 
            where TComponent : TService
        {
            container.RegisterTransient(typeof(TService), typeof(TComponent));
        }

        public static void Register<TService>(this IServiceContainer container, TService instance) {
            container.RegisterInstance(instance);
        }

        public static void RegisterInstance<TService>(this IServiceContainer container, TService instance) {
            container.RegisterInstance(typeof(TService), instance);
        }

        public static void RegisterInstance(this IServiceContainer container, object instance) {
            container.RegisterInstance(instance.GetType(), instance);
        }
    }
}
