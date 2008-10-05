using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Abstraction {
    public interface IComponentFactory {
        object CreateInstance(Type componentType);
    }
}
