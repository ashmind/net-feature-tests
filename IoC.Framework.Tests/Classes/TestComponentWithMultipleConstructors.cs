using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Classes {
    public class TestComponentWithMultipleConstructors {
        public static class ConstructorNames {
            public const string Default = "Default";
            public const string MostResolvable = "Most resolvable";
            public const string MostDefined = "Most defined";
            public const string Unresolvable = "Unresolvable";
        }

        public TestComponentWithMultipleConstructors() {
            this.UsedConstructorName = ConstructorNames.Default;
        }

        public TestComponentWithMultipleConstructors(ITestService service) {
            this.UsedConstructorName = ConstructorNames.MostResolvable;
        }

        public TestComponentWithMultipleConstructors(IUnregisteredTestService service) {
            this.UsedConstructorName = ConstructorNames.Unresolvable;
        }

        public TestComponentWithMultipleConstructors(ITestService service1, IUnregisteredTestService service2) {
            this.UsedConstructorName = ConstructorNames.MostDefined;
        }

        public string UsedConstructorName { get; set; }
    }
}
