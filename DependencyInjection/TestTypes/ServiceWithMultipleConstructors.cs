using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.FeatureTests.TestTypes {
    public class ServiceWithMultipleConstructors {
        public static class ConstructorNames {
            public const string Default = "Default";
            public const string MostResolvable = "Most resolvable";
            public const string MostDefined = "Most defined";
            public const string Unresolvable = "Unresolvable";
        }

        public ServiceWithMultipleConstructors() {
            this.UsedConstructorName = ConstructorNames.Default;
        }

        public ServiceWithMultipleConstructors(IService service) {
            this.UsedConstructorName = ConstructorNames.MostResolvable;
        }

        public ServiceWithMultipleConstructors(IUnregisteredService service) {
            this.UsedConstructorName = ConstructorNames.Unresolvable;
        }

        public ServiceWithMultipleConstructors(IService service1, IUnregisteredService service2) {
            this.UsedConstructorName = ConstructorNames.MostDefined;
        }

        public string UsedConstructorName { get; set; }
    }
}
