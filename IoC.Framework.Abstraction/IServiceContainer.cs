using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public interface IServiceContainer {
        void RegisterSingleton(Type serviceType, Type componentType);
        void RegisterTransient(Type serviceType, Type componentType);
        void RegisterInstance(Type serviceType, object instance);
    }
}
