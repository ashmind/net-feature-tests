using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class Wrapper<T> {
        public Wrapper() {
            this.Value = NextValueAfterConstructor;
            NextValueAfterConstructor = default(T);
        }

        public T Value { get; set; }
        public static T NextValueAfterConstructor { get; set; }
    }
}
