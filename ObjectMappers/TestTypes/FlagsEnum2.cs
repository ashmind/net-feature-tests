using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    [Flags]
    public enum FlagsEnum2 {
        First  = FlagsEnum1.First * 2,
        Second = FlagsEnum1.Second * 2
    }
}
