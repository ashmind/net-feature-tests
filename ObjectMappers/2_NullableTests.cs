using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(2)]
    [DisplayName("Nullable types")]
    [Description("_TODO_")]
    public class NullableTests {
        [Feature]
        [DisplayName("int ⇒ int?")]
        public void Int32ToNullableInt32(IObjectMapperAdapter mapper) {
            AssertNullableMappingWorksFor(mapper, 5, (int?)5);
        }

        [Feature]
        [DisplayName("int ⇒ long?")]
        public void Int32ToNullableInt64(IObjectMapperAdapter mapper) {
            AssertNullableMappingWorksFor(mapper, 5, (long?)5);
        }

        [Feature]
        [DisplayName("int? ⇒ long?")]
        public void NullableInt32ToNullableInt64(IObjectMapperAdapter mapper) {
            AssertNullableMappingWorksFor(mapper, (int?)5, (long?)5);
        }
        
        [Feature]
        [DisplayName("(int?)null ⇒ (object)null")]
        public void NullNullableInt32ToNullObject(IObjectMapperAdapter mapper) {
            AssertNullableMappingWorksFor(mapper, (int?)null, (object)null, "not set");
        }

        [Feature]
        [DisplayName("(object)null ⇒ (int?)null")]
        public void NullObjectToNullNullableInt32(IObjectMapperAdapter mapper) {
            AssertNullableMappingWorksFor(mapper, (object)null, (int?)null, 1);
        }

        [Feature]
        [DisplayName("DateTime ⇒ DateTimeOffset?")]
        public void DateTimeToNullableDateTimeOffset(IObjectMapperAdapter mapper) {
            var date = new DateTime(635522607077343456);
            AssertNullableMappingWorksFor(mapper, date, (DateTimeOffset?)date);
        }

        [Feature]
        [DisplayName("DateTime? ⇒ DateTimeOffset?")]
        public void NullableDateTimeToNullableDateTimeOffset(IObjectMapperAdapter mapper) {
            var date = new DateTime(635522607077343456);
            AssertNullableMappingWorksFor(mapper, (DateTime?)date, (DateTimeOffset?)date);
        }

        private static void AssertNullableMappingWorksFor<TSource, TTarget>(IObjectMapperAdapter mapper, TSource sourceValue, TTarget expectedValue, TTarget targetValue = default(TTarget)) {
            mapper.CreateMap<Wrapper<TSource>, Wrapper<TTarget>>();

            var source = new Wrapper<TSource> { Value = sourceValue };
            WrapperStatic<TTarget>.NextValueAfterConstructor = targetValue;

            var result = mapper.Map<Wrapper<TTarget>>(source);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}