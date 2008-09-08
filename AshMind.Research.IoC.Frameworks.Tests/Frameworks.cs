using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Research.IoC.Frameworks.Tests.Adapters;
using MbUnit.Framework;

namespace AshMind.Research.IoC.Frameworks.Tests {
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
