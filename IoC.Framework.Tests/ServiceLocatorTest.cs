using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Abstraction;
using IoC.Framework.Test.Classes;

namespace IoC.Framework.Tests {
    [TypeFixture(typeof(IServiceInjectionFramework))]
    [ProviderFactory(typeof(FrameworkFactory), typeof(IServiceInjectionFramework))]
    public class ServiceLocatorTest {
        [Test]
        public void TestSupportsKeysForTransients(IServiceInjectionFramework framework) {
            AssertSupportsKeysFor(framework, container => container.AddTransient);
        }

        [Test]
        public void TestSupportsKeysForSingletons(IServiceInjectionFramework framework) {
            AssertSupportsKeysFor(framework, container => container.AddSingleton);
        }

        [Test]
        public void TestSupportsKeysForInstances(IServiceInjectionFramework framework) {
            var first = new IndependentTestComponent();
            var second = new IndependentTestComponent();

            var container = framework.CreateContainer();
            container.AddInstance<ITestService>(first, "FirstKey");
            container.AddInstance<ITestService>(second, "SecondKey");

            var locator = framework.CreateLocator(container);
            var resolved = locator.GetInstance<ITestService>("FirstKey");

            Assert.AreSame(first, resolved);
        }

        public void AssertSupportsKeysFor(IServiceInjectionFramework framework, Func<IServiceContainer, Action<Type, Type, string>> getAdd) {
            var container = framework.CreateContainer();
            var add = getAdd(container);

            add(typeof(ITestService), typeof(IndependentTestComponent), "FirstKey");
            add(typeof(ITestService), typeof(AnotherIndependentTestComponent), "SecondKey");

            var locator = framework.CreateLocator(container);
            var firstComponent = locator.GetInstance<ITestService>("FirstKey");
            var secondComponent = locator.GetInstance<ITestService>("SecondKey");

            Assert.IsNotNull(firstComponent);
            Assert.IsNotNull(secondComponent);

            Assert.IsInstanceOfType(typeof(IndependentTestComponent), firstComponent);
            Assert.IsInstanceOfType(typeof(AnotherIndependentTestComponent), secondComponent);
        }
    }
}
