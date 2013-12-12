using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ClassWithGetValueMethod {
        private readonly string value;

        public ClassWithGetValueMethod(string value) {
            this.value = value;
        }

        public string GetValue() {
            return this.value;
        }
    }
}
