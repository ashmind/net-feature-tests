using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;
using IoC.Framework.Castle;

using MbUnit.Framework;

namespace IoC.Framework.Tests {
    public class FrameworkFactory {
        [Factory]
        public IServiceInjectionFramework Castle {
            get { return new CastleFramework(); }
        }
    }
}
