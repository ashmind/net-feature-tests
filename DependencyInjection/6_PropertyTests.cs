using System.ComponentModel;
using System.Linq;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using Xunit;
using DependencyInjection.FeatureTests.TestTypes;
using FeatureTests.On.DependencyInjection.Adapters;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection
{
    [DisplayOrder(6)]
    [DisplayName("Property dependencies")]
    [Description(@"
        While property dependencies are a nice feature, it is rarely a good practice to actually use them.
   
        I have not yet decided whether this table is worth keeping, so do not assign much weight to these tests.     
    ")]
    [FeatureScoring(FeatureScoring.NotScored)]
    [SpecialCase(typeof(SimpleInjectorAdapter), @"
        Simple Injector does not inject properties out of the box, but this behavior 
        can be changed by replacing the Container.Options.PropertySelectionBehavior.
        For more info see: https://bit.ly/1cmMxuS.
    ", Skip = true)]
    public class PropertyTests {
        [Feature]
        [DisplayName("Simple dependency")]
        public void PropertyDependency(IAdapter adapter) {
            adapter.Register<IService, IndependentService>();
            adapter.Register<ServiceWithSimplePropertyDependency>();

            var component = adapter.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [Feature]
        [DependsOnFeature("PropertyDependency")]
        [DisplayName("Optional by default")]
        public void PropertyDependencyIsOptional(IAdapter adapter) {
            adapter.Register<ServiceWithSimplePropertyDependency>();
            var component = adapter.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.Null(component.Service);
        }

        [Feature]
        [DependsOnFeature("PropertyDependency")]
        [DisplayName("Dependency without attributes")]
        [Description(@"
            Normally, it is undesirable to reference DI framework from the most parts of the code.  
            Requirement to use explicit attributes contributes to this problem.
        ")]
        public void PropertyDependencyDoesNotNeedCustomAttribute(IAdapter adapter) {
            var property = typeof(ServiceWithSimplePropertyDependency).GetProperty("Service");
            var attributes = property.GetCustomAttributes(false);
            var attributesFromThisFramework = attributes.Where(a => a.GetType().Assembly == adapter.Assembly);

            Assert.Empty(attributesFromThisFramework);
        }
    }
}
