using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Feature.Tests.Adapters;

namespace IoC.Framework.Feature.Tests {
    [TypeFixture(typeof(IFrameworkAdapter))]
    [ProviderFactory(typeof(Frameworks), typeof(IFrameworkAdapter))]
    public abstract class FrameworkTestBase {
    }
}
