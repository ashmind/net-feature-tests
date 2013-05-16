using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests {
    [DisplayOrder(5)]
    [DisplayName("Property dependencies")]
    [SpecialCase(typeof(SimpleInjectorAdapter), @"
        Simple Injector does not inject properties out of the box, but this behavior 
        can be changed by replacing the Container.Options.PropertySelectionBehavior.
    ", Skip = true)]
    public class PropertyTests
    {
        [Feature]
        [DisplayName("Simple dependency")]
        public void PropertyDependency(IFrameworkAdapter framework) {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimplePropertyDependency>();

            var component = framework.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [Feature]
        [DisplayName("Optional by default")]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework) {
            framework.Register<ServiceWithSimplePropertyDependency>();
            var component = framework.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.Null(component.Service);
        }
    }
}
