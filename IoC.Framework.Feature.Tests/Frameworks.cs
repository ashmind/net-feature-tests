using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Castle;
using IoC.Framework.Feature.Tests.Adapters;

namespace IoC.Framework.Feature.Tests {
    public class Frameworks {
        [Factory]
        public IFrameworkAdapter Autofac {
            get { return new AutofacAdapter(); }
        }

        [Factory]
        public IFrameworkAdapter Castle {
            get { return new DefaultAdapter(new CastleFramework(), false); }
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
            get { return new DefaultAdapter(new CastleFramework(), true); }
        }
    }
}
