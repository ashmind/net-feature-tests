using System.ComponentModel;
using System.Linq;
using Xunit;
using FeatureTests.On.DependencyInjection.Adapters.Interface;
using FeatureTests.On.DependencyInjection.TestTypes;
using FeatureTests.Shared;

namespace FeatureTests.On.DependencyInjection {
    [DisplayOrder(4)]
    [DisplayName("Optional parameters")]
    [Description(@"
        Optional parameters can be used to specify an optional dependency, e.g.
        
        `Service(IService required, IOptionalService optional = null)`
    ")]
    [FeatureScoring(FeatureScoring.SinglePoint)]
    public class OptionalParameterTests {
        [Feature]
        [DisplayName("Missing primitive")]
        [Description("An `int` optional parameter, with no `int` values in container.")]
        public void MissingPrimitive(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithDependencyAndOptionalInt32Parameter>();

            var component = adapter.Resolve<ServiceWithDependencyAndOptionalInt32Parameter>();

            Assert.NotNull(component);
        }

        [Feature]
        [DependsOnFeature("MissingPrimitive")]
        [DisplayName("Missing primitive with default value")]
        [Description("An `int` optional parameter with a default value that should be used.")]
        public void MissingPrimitiveDefaultValue(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithDependencyAndOptionalInt32Parameter>();

            var component = adapter.Resolve<ServiceWithDependencyAndOptionalInt32Parameter>();

            Assert.Equal(5, component.Optional);
        }

        [Feature]
        [DisplayName("Missing dependency")]
        [Description("An interface optional parameter, with no matching registrations in container.")]
        public void MissingDependency(IContainerAdapter adapter) {
            adapter.RegisterType<IService, IndependentService>();
            adapter.RegisterType<ServiceWithDependencyAndOptionalOtherServiceParameter>();

            var component = adapter.Resolve<ServiceWithDependencyAndOptionalOtherServiceParameter>();

            Assert.NotNull(component);
            Assert.Null(component.Optional);
        }
    }
}
