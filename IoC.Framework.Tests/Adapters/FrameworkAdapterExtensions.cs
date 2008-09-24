using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Adapters {
    public static class FrameworkAdapterExtensions {
        public static void Register<TComponent, TService>(this IFrameworkAdapter adapter) 
            where TComponent : TService
        {
            adapter.RegisterTransient<TComponent, TService>();
        }

        public static void Register<TComponent>(this IFrameworkAdapter adapter) {
            adapter.Register<TComponent, TComponent>();
        }
    }
}
