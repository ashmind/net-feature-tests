using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ClassWithSetValue {
        private string value;

        public void SetValue(string value) {
            this.value = value;
        }

        public string GetValue() {
            return this.value;
        }
    }
}
