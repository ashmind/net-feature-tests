using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using MbUnit.Framework;
using AshMind.Research.IoC.Frameworks.Tests.Adapters;
using AshMind.Research.IoC.Frameworks.Tests.Classes;

namespace AshMind.Research.IoC.Frameworks.Tests {
    public class MustHaveTest : FrameworkTestBase {
        [Test]
        public void ResolvesJustRegisteredService(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();
            var component = framework.Resolve<ITestService>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolvesServiceJustRegisteredAsItself(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent>();
            var component = framework.Resolve<IndependentTestComponent>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void SupportsSingletons(IFrameworkAdapter framework) {
            framework.RegisterSingleton<IndependentTestComponent, ITestService>();
            var instance1 = framework.Resolve<ITestService>();
            var instance2 = framework.Resolve<ITestService>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void SupportsTransients(IFrameworkAdapter framework) {
            framework.RegisterTransient<IndependentTestComponent, ITestService>();
            var instance1 = framework.Resolve<ITestService>();
            var instance2 = framework.Resolve<ITestService>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void SupportsInstanceResolution(IFrameworkAdapter framework) {
            var instance = new IndependentTestComponent();
            framework.Register<ITestService>(instance);
            var resolved = framework.Resolve<ITestService>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void SupportsInstanceResolutionForDependency(IFrameworkAdapter framework) {
            var instance = new IndependentTestComponent();
            framework.Register<ITestService>(instance);
            framework.Register<TestComponentWithSimpleConstructorDependency>();
            var dependent = framework.Resolve<TestComponentWithSimpleConstructorDependency>();

            Assert.AreSame(instance, dependent.Service);
        }

        [Test]
        public void SupportsConstructorDependency(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();
            framework.Register<TestComponentWithSimpleConstructorDependency>();

            var component = framework.Resolve<TestComponentWithSimpleConstructorDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), component.Service);
        }

        [Test]
        public void SupportsPropertyDependency(IFrameworkAdapter framework) {
            framework.Register<IndependentTestComponent, ITestService>();
            framework.Register<TestComponentWithSimplePropertyDependency>();

            var component = framework.Resolve<TestComponentWithSimplePropertyDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentTestComponent), component.Service);
        }
    }
}