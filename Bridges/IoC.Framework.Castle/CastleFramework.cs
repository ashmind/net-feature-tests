using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

using IoC.Framework.Abstraction;

namespace IoC.Framework.Castle {
    public class CastleFramework : IServiceInjectionFramework {
        public IServiceContainer CreateContainer() {
            return new CastleContainer();
        }

        public IServiceLocator CreateLocator(IServiceContainer container) {
            return (CastleContainer)container;
        }
    }
}
