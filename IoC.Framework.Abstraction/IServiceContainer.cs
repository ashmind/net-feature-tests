using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public interface IServiceContainer {
        void AddSingleton(Type serviceType, Type componentType, string key);
        void AddTransient(Type serviceType, Type componentType, string key);
        void AddInstance(Type serviceType, object instance, string key);
    }
}
