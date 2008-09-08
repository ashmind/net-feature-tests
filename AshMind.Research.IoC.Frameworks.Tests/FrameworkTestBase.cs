using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;
using AshMind.Research.IoC.Frameworks.Tests.Adapters;

namespace AshMind.Research.IoC.Frameworks.Tests {
    [TypeFixture(typeof(IFrameworkAdapter))]
    [ProviderFactory(typeof(Frameworks), typeof(IFrameworkAdapter))]
    public abstract class FrameworkTestBase {
    }
}
