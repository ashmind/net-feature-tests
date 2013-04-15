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
    [DisplayName("List/array dependencies")]
    [Description(@"
        When several registrations of IService exist in a container, many
        frameworks automatically provide IService[] (or List<IService> etc).

        This is extremely important for open/closed principle. For example,
        UserValidator can have dependency on IUserValidationRule[]. Then new
        rules can be transparently added to the system without any changes to
        UserValidator.
    ")]
    [SpecialCase(typeof(UnityAdapter), "Note: Unity requires named registrations for list resolution to work.")]
    public class ListTests {
        [DisplayName("IService[]")]
        [ForEachFramework]
        public void Array(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IService[]>>(framework);
        }

        [DisplayName("IList<IService>")]
        [ForEachFramework]
        public void List(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IList<IService>>>(framework);
        }

        [DisplayName("ICollection<IService>")]
        [ForEachFramework]
        public void Collection(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<ICollection<IService>>>(framework);
        }

        [DisplayName("IEnumerable<IService>")]
        [ForEachFramework]
        public void Enumerable(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IEnumerable<IService>>>(framework);
        }
        
        public void AssertResolvesListDependencyFor<TTestComponent>(IFrameworkAdapter framework)
            where TTestComponent : IServiceWithListDependency<IEnumerable<IService>>
        {
            framework.Register<IService, IndependentService>();
            framework.Register<TTestComponent>();

            var resolved = framework.Resolve<TTestComponent>();

            Assert.NotNull(resolved);
            Assert.NotNull(resolved.Services);
            Assert.Equal(1, resolved.Services.Count());
            Assert.IsAssignableFrom<IndependentService>(resolved.Services.First());
        }
    }
}
