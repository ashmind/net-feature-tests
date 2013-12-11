using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ClassWithNested<TNested>
        where TNested : new()
    {
        public ClassWithNested() {
            this.Nested = new TNested();
        }

        public TNested Nested { get; set; }
    }
}
