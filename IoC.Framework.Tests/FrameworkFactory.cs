using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Abstraction;
using IoC.Framework.Castle;
using IoC.Framework.Unity;

namespace IoC.Framework.Tests {
    public class FrameworkFactory {
        [Factory]
        public IServiceInjectionFramework Castle {
            get { return new CastleFramework(); }
        }

        [Factory]
        public IServiceInjectionFramework Unity {
            get { return new UnityFramework(); }
        }
    }
}
