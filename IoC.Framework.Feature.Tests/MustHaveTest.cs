using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using IoC.Framework.Abstraction;
using IoC.Framework.Feature.Tests.Adapters;
using IoC.Framework.Feature.Tests.Classes;

namespace IoC.Framework.Feature.Tests {
    public class MustHaveTest : FrameworkTestBase {
        [Test]
        public void ResolvesJustRegisteredService(IFrameworkAdapter framework) {
            framework.Add<ITestService, IndependentTestComponent>();
            var component = framework.GetInstance<ITestService>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolvesServiceJustRegisteredAsItself(IFrameworkAdapter framework) {
            framework.Add<IndependentTestComponent>();
            var component = framework.GetInstance<IndependentTestComponent>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void SupportsSingletons(IFrameworkAdapter framework) {
            framework.AddSingleton<ITestService, IndependentTestComponent>();
            var instance1 = framework.GetInstance<ITestService>();
            var instance2 = framework.GetInstance<ITestService>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void SupportsTransients(IFrameworkAdapter framework) {
            framework.AddTransient<ITestService, IndependentTestComponent>();
            var instance1 = framework.GetInstance<ITestService>();
            var instance2 = framework.GetInstance<ITestService>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void SupportsInstanceResolution(IFrameworkAdapter framework) {
            var instance = new IndependentTestComponent();
            framework.Add<ITestService>(instance);

            var resolved = framework.GetInstance<ITestService>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void SupportsInstanceResolutionForDependency(IFrameworkAdapter framework) {
            var instance = new IndependentTestComponent();
            framework.Add<ITestService>(instance);
            framework.Add<TestComponentWithSimpleConstructorDependency>();

            var dependent = framework.GetInstance<TestComponentWithSimpleConstructorDependency>();

            Assert.AreSame(instance, dependent.Service);
        }

        [Test]
        public void SupportsConstructorDependency(IFrameworkAdapter framework) {
            framework.Add<ITestService, IndependentTestComponent>();
            framework.Add<TestComponentWithSimpleConstructorDependency>();

            var component = framework.GetInstance<TestComponentWithSimpleConstructorDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), component.Service);
        }

        [Test]
        public void SupportsPropertyDependency(IFrameworkAdapter framework) {
            framework.Add<ITestService, IndependentTestComponent>();
            framework.Add<TestComponentWithSimplePropertyDependency>();

            var component = framework.GetInstance<TestComponentWithSimplePropertyDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), component.Service);
        }
    }
}