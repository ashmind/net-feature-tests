using System;
using System.Collections.Generic;
using System.Linq;

using IoC.Framework.Abstraction;

using MbUnit.Framework;

namespace IoC.Framework.Tests {
    [TypeFixture(typeof(IServiceInjectionFramework))]
    [ProviderFactory(typeof(FrameworkFactory), typeof(IServiceInjectionFramework))]
    public class ServiceLocatorTest {

    }
}
