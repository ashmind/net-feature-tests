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
    [DisplayOrder(2)]
    [DisplayName("List/array dependency support")]
    [Description(@"
        When several registrations of IService exist in a container, many
        frameworks automatically provide TService[] (or List<TService> etc).

        This is extremely important for open/closed principle. For example,
        UserValidator can have dependency on IUserValidationRule[]. Then new
        rules can be transparently added to the system without any changes to
        UserValidator.
    ")]
    public class ListTests {
        [DisplayName("Constructor dependency")]
        [ForEachFramework]
        public void ListConstructorDependency(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithArrayConstructorDependency>(framework);
        }

        [DisplayName("Property dependency")]
        [ForEachFramework]
        public void ListPropertyDependency(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithArrayPropertyDependency>(framework);
        }

        public void AssertResolvesListDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : IServiceWithArrayDependency {
            framework.Register<IService, IndependentService>();
            framework.Register<TTestComponent>();

            var resolved = framework.GetInstance<TTestComponent>();

            Assert.NotNull(resolved);
            Assert.NotNull(resolved.Services);
            Assert.Equal(1, resolved.Services.Length);
            Assert.IsAssignableFrom<IndependentService>(resolved.Services[0]);
        }
    }
}
