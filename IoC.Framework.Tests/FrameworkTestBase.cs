using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Tests.Adapters;

namespace IoC.Framework.Tests {
    [TypeFixture(typeof(IFrameworkAdapter))]
    [ProviderFactory(typeof(Frameworks), typeof(IFrameworkAdapter))]
    public abstract class FrameworkTestBase {
    }
}
