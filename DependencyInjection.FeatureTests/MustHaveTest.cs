using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.TestTypes;
using MbUnit.Framework;

namespace DependencyInjection.FeatureTests {
    public class MustHaveTest : FrameworkTestBase {
        [Test]
        public void ResolvesJustRegisteredService(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            var component = framework.GetInstance<IEmptyService>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolvesServiceJustRegisteredAsItself(IFrameworkAdapter framework) {
            framework.Add<IndependentService>();
            var component = framework.GetInstance<IndependentService>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void SupportsSingletons(IFrameworkAdapter framework) {
            framework.AddSingleton<IEmptyService, IndependentService>();
            var instance1 = framework.GetInstance<IEmptyService>();
            var instance2 = framework.GetInstance<IEmptyService>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void SupportsTransients(IFrameworkAdapter framework) {
            framework.AddTransient<IEmptyService, IndependentService>();
            var instance1 = framework.GetInstance<IEmptyService>();
            var instance2 = framework.GetInstance<IEmptyService>();

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void SupportsInstanceResolution(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Add<IEmptyService>(instance);

            var resolved = framework.GetInstance<IEmptyService>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void SupportsInstanceResolutionForDependency(IFrameworkAdapter framework) {
            var instance = new IndependentService();
            framework.Add<IEmptyService>(instance);
            framework.Add<ServiceWithSimpleConstructorDependency>();

            var dependent = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.AreSame(instance, dependent.Service);
        }

        [Test]
        public void SupportsConstructorDependency(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithSimpleConstructorDependency>();

            var component = framework.GetInstance<ServiceWithSimpleConstructorDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentService), component.Service);
        }

        [Test]
        public void SupportsPropertyDependency(IFrameworkAdapter framework) {
            framework.Add<IEmptyService, IndependentService>();
            framework.Add<ServiceWithSimplePropertyDependency>();

            var component = framework.GetInstance<ServiceWithSimplePropertyDependency>();

            Assert.IsNotNull(component.Service);
            Assert.IsInstanceOfType(typeof(IndependentService), component.Service);
        }
    }
}