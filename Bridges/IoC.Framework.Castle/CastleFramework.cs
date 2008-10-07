using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;
using Microsoft.Practices.ServiceLocation;

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
