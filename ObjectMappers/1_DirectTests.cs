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
        [DisplayName("string ⇒ int")]
        public void StringToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, "123", 123);
        }

        [Feature]
        [DisplayName("object ⇒ int")]
        public void ObjectToInt32(IObjectMapperAdapter mapper) {
            AssertPrimitiveMappingWorksFor(mapper, (object)123, 123);
        }

        private static void AssertPrimitiveMappingWorksFor<TSource, TTarget>(IObjectMapperAdapter mapper, TSource sourceValue, TTarget expectedValue) {
            mapper.CreateMap<ObjectWithSingleProperty<TSource>, ObjectWithSingleProperty<TTarget>>();

            var source = new ObjectWithSingleProperty<TSource> { Value = sourceValue };
            var result = mapper.Map<ObjectWithSingleProperty<TTarget>>(source);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}