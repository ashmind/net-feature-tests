using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class Wrapper<T> {
        public Wrapper() {
            this.Value = WrapperStatic<T>.NextValueAfterConstructor;
            WrapperStatic<T>.NextValueAfterConstructor = default(T);
        }

        public T Value { get; set; }
    }

    // only moved into a separate class because of fFastMapper 
    // failing if class has any static properties
    public static class WrapperStatic<T> {
        public static T NextValueAfterConstructor { get; set; }
    }
}
