using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;
using Microsoft.Practices.ServiceLocation;

namespace IoC.Framework.Unity {
    public class UnityFramework : IServiceInjectionFramework {
        public IServiceContainer CreateContainer() {
            return new UnityContainerAdapter();
        }

        public IServiceLocator CreateLocator(IServiceContainer container) {
            return (UnityContainerAdapter)container;
        }
    }
}
