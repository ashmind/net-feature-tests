using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public static class ComponentFactoryExtensions {
        public static TComponent CreateInstance<TComponent>(this IComponentFactory factory) {
            return (TComponent)factory.CreateInstance(typeof(TComponent));
        }
    }
}
