using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ImmutableWrapper<T> {
        private readonly T value;

        public ImmutableWrapper(T value) {
            this.value = value;
        }

        public T Value {
            get { return this.value; }
        }
    }
}
