using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(1)]
    [DisplayName("Primitive conversion")]
    [Description("The simplest test: mapping between primitive values.")]
    public class PrimitiveConversionTests {
        [Feature]
        [DisplayName("string ⇒ string")]
        public void StringToString(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, "ABC", "ABC");
        }

        [Feature]
        [DisplayName("int ⇒ int")]
        public void Int32ToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, 123, 123);
        }

        [Feature]
        [DisplayName("int ⇒ object")]
        public void Int32ToObject(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, 123, (object)123);
        }

        [Feature]
        [DisplayName("int ⇒ string")]
        public void Int32ToString(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, 123, "123");
        }
        
        [Feature]
        [DisplayName("int ⇒ long")]
        public void Int32ToInt64(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, 123, 123L);
        }

        [Feature]
        [DisplayName("int ⇒ decimal")]
        public void Int32ToDecimal(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, 123, (decimal)123);
        }

        [Feature]
        [DisplayName("byte ⇒ int")]
        public void ByteToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, (byte)123, 123);
        }

        [Feature]
        [DisplayName("short ⇒ int")]
        public void Int16ToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, (short)123, 123);
        }

        [Feature]
        [DisplayName("string ⇒ int")]
        public void StringToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, "123", 123);
        }

        [Feature]
        [DisplayName("object ⇒ int")]
        public void ObjectToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, (object)123, 123);
        }

        [Feature]
        [DisplayName("DateTime ⇒ DateTime")]
        public void DateTimeToDateTime(IObjectMapperAdapter mapper) {
            var now = DateTime.Now;
            AssertPrimitiveMappingWorksFor(mapper, now, now);
        }

        [Feature]
        [DisplayName("DateTime ⇒ DateTimeOffset")]
        public void DateTimeToDateTimeOffset(IObjectMapperAdapter mapper) {
            var now = DateTime.Now;
            AssertPrimitiveMappingWorksFor(mapper, now, (DateTimeOffset)now);
        }
        
        private static void AssertPrimitiveMappingWorksFor<TSource, TTarget>(IObjectMapperAdapter mapper, TSource sourceValue, TTarget expectedValue) {
            mapper.CreateMap<Wrapper<TSource>, Wrapper<TTarget>>();

            var source = new Wrapper<TSource> { Value = sourceValue };
            var result = mapper.Map<Wrapper<TTarget>>(source);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}
