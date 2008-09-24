using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Tests.Adapters;

using MbUnit.Framework;

namespace IoC.Framework.Tests {
    public class Frameworks {
        [Factory]
        public IFrameworkAdapter Autofac {
            get { return new AutofacAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter Castle {
            get { return new CastleAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter LinFu
        {
            get { return new LinFuAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter Ninject {
            get { return new NinjectAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter Spring {
            get { return new SpringAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter StructureMap {
            get { return new StructureMapAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter Unity {
            get { return new UnityAdapter(); }
        }
    }
}
