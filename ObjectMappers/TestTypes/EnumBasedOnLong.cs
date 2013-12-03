using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public enum EnumBasedOnLong : long {
        Default = Enum2.Default + 10
    }
}
