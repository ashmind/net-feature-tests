using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Abstraction {
    public interface IServiceInjectionFramework {
        IServiceContainer CreateContainer();
        IServiceLocator CreateLocator(IServiceContainer container);
    }
}
