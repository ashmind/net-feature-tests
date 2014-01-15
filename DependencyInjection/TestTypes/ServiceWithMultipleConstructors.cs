using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.DependencyInjection.TestTypes {
    public class ServiceWithMultipleConstructors {
        public static class ConstructorNames {
            public const string Default = "Default";
            public const string MostResolvable = "Most resolvable";
            public const string MostDefined = "Most defined";
            public const string Unresolvable = "Unresolvable";
        }

        private readonly string usedConstructorName;

        public ServiceWithMultipleConstructors() {
            this.usedConstructorName = ConstructorNames.Default;
        }

        public ServiceWithMultipleConstructors(IService service) {
            this.usedConstructorName = ConstructorNames.MostResolvable;
        }

        public ServiceWithMultipleConstructors(IUnregisteredService service) {
            this.usedConstructorName = ConstructorNames.Unresolvable;
        }

        public ServiceWithMultipleConstructors(IService service1, IUnregisteredService service2) {
            this.usedConstructorName = ConstructorNames.MostDefined;
        }

        public string UsedConstructorName {
            get { return this.usedConstructorName; }
        }
    }
}
