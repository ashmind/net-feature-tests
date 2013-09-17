using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests
{
    [DisplayOrder(2)]
    [DisplayName("List/array dependencies")]
    [Description(@"
        When several registrations of IService exist in a container, many
        frameworks automatically provide IService[] (or List<IService> etc).

        This is extremely important for open/closed principle. For example,
        UserValidator can have dependency on IUserValidationRule[]. Then new
        rules can be transparently added to the system without any changes to
        UserValidator.

        @dotnetjunkie:
        .NET 4.5 introduced two new collection interfaces IReadOnlyCollection<T>
        and IReadOnlyList<T> that define 'finite iterator' behavior and DI
        frameworks should natively support these interface to prevent
        defensive copying of arrays (as Mark Seemann explains here 
        http://blog.ploeh.dk/2013/07/20/linq-versus-the-lsp/).
    ")]
    [FeatureScoring(FeatureScoring.PointPerClass)]
    public class ListTests {
        [Feature]
        [DisplayName("IService[]")]
        [SpecialCase(typeof(UnityAdapter), "Note: Unity requires named registrations for list resolution to work.")]
        public void Array(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IService[]>>(framework);
        }

        [Feature]
        [DisplayName("IList<IService>")]
        public void List(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IList<IService>>>(framework);
        }

        [Feature]
        [DisplayName("ICollection<IService>")]
        public void Collection(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<ICollection<IService>>>(framework);
        }

        [Feature]
        [DisplayName("IEnumerable<IService>")]
        [SpecialCase(typeof(SimpleInjectorAdapter), @"
            Simple Injector does support resolving IEnumerable<T>, but requires a single registration 
            using one of the RegisterAll methods to register all types at once, which is a different
            strategy than the other frameworks use.
        ", Skip = false)]
        public void Enumerable(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IEnumerable<IService>>>(framework);
        }

        [Feature]
        [DisplayName("IReadOnlyCollection<IService>")]
        public void IReadOnlyCollection(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyCollection<IService>>>(framework);
        }

        [Feature]
        [DisplayName("IReadOnlyList<IService>")]
        public void IReadOnlyList(IFrameworkAdapter framework) {
            this.AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyList<IService>>>(framework);
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
