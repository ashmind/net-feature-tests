using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(3)]
    [DisplayName("Enums")]
    public class EnumTests {
        [Feature]
        [DisplayName("Enum₁ ⇒ Enum₂ (same name)")]
        public void EnumToEnumSameItemName(IObjectMapperAdapter mapper) {
            AssertEnumMappingWorksFor(mapper, Enum1.Default, Enum2.Default);
        }

        [Feature]
        [DisplayName("Enum₁ ⇒ Enum₂ (flags)")]
        public void EnumToEnumFlags(IObjectMapperAdapter mapper) {
            AssertEnumMappingWorksFor(mapper, FlagsEnum1.First | FlagsEnum1.Second, FlagsEnum2.First | FlagsEnum2.Second);
        }

        [Feature]
        [DisplayName("Enum₁:long ⇒ Enum₂:int")]
        public void EnumInt64ToEnumInt32(IObjectMapperAdapter mapper) {
            AssertEnumMappingWorksFor(mapper, EnumBasedOnLong.Default, Enum2.Default);
        }

        [Feature]
        [DisplayName("Enum₁? ⇒ Enum₂")]
        public void EnumToNullableEnum(IObjectMapperAdapter mapper) {
            AssertEnumMappingWorksFor(mapper, (Enum1?)Enum1.Default, Enum2.Default);
        }

        [Feature]
        [DisplayName("string ⇒ Enum")]
        public void StringToEnum(IObjectMapperAdapter mapper) {
            AssertEnumMappingWorksFor(mapper, "Default", Enum2.Default);
        }

        private static void AssertEnumMappingWorksFor<TSource, TTarget>(IObjectMapperAdapter mapper, TSource sourceValue, TTarget expectedValue) {
            mapper.CreateMap<Wrapper<TSource>, Wrapper<TTarget>>();

            var source = new Wrapper<TSource> { Value = sourceValue };
            var result = mapper.Map<Wrapper<TTarget>>(source);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}