using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using MbUnit.Framework;

namespace DependencyInjection.FeatureTests {
    [TypeFixture(typeof(IFrameworkAdapter))]
    [ProviderFactory(typeof(Frameworks), typeof(IFrameworkAdapter))]
    public abstract class FrameworkTestBase {
    }
}
