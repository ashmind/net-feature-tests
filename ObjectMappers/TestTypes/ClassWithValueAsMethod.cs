using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ClassWithValueAsMethod {
        private readonly string value;

        public ClassWithValueAsMethod(string value) {
            this.value = value;
        }

        public string Value() {
            return this.value;
        }
    }
}
