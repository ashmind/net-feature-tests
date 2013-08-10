using System.ComponentModel;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.TestTypes;
using DependencyInjection.FeatureTests.XunitSupport;
using Xunit;

namespace DependencyInjection.FeatureTests
{
    [DisplayOrder(5)]
    [DisplayName("Property dependencies")]
    [Description(@"
        While property dependencies are a nice feature, it is rarely a good practice to actually use them.
   
        I have not yet decided whether this table is worth keeping, so do not assign much weight to these tests.     
    ")]
    [SpecialCase(typeof(SimpleInjectorAdapter), @"
        Simple Injector does not inject properties out of the box, but this behavior 
        can be changed by replacing the Container.Options.PropertySelectionBehavior.
        For more info see: https://bit.ly/1cmMxuS.
    ", Skip = true)]
    public class PropertyTests
    {
        [Feature]
        [DisplayName("Simple dependency")]
        public void PropertyDependency(IFrameworkAdapter framework)
        {
            framework.Register<IService, IndependentService>();
            framework.Register<ServiceWithSimplePropertyDependency>();

            var component = framework.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.NotNull(component.Service);
            Assert.IsAssignableFrom<IndependentService>(component.Service);
        }

        [Feature]
        [DisplayName("Optional by default")]
        public void PropertyDependencyIsOptional(IFrameworkAdapter framework)
        {
            framework.Register<ServiceWithSimplePropertyDependency>();
            var component = framework.Resolve<ServiceWithSimplePropertyDependency>();

            Assert.Null(component.Service);
        }

        [Feature]
        [DependsOnFeature("PropertyDependency")]
        [DisplayName("Dependency without attributes")]
        [Description(@"
            Normally, it is undesirable to reference DI framework from the most parts of the code.  
            Requirement to use explicit attributes contributes to this problem.
        ")]
        public void PropertyDependencyDoesNotNeedCustomAttribute(IFrameworkAdapter framework)
        {
            var property = typeof(ServiceWithSimplePropertyDependency).GetProperty("Service");
            var attributes = property.GetCustomAttributes(false);
            var attributesFromThisFramework = attributes.Where(a => a.GetType().Assembly == framework.FrameworkAssembly);

            Assert.Empty(attributesFromThisFramework);
        }
    }
}
