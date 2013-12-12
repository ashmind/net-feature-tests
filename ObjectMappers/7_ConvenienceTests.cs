using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(7)]
    [DisplayName("Convenience")]
    public class ConvenienceTests {
        [Feature]
        [DisplayName("Automatic flattening")]
        public void NameBasedFlattening(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithNested<ClassWithValue>, ClassWithNestedValue>();
            var source = new ClassWithNested<ClassWithValue> { Nested = { Value = "ABC" } };
            var result = adapter.Map<ClassWithNestedValue>(source);

            Assert.Equal(source.Nested.Value, result.NestedValue);
        }

        [Feature]
        [DisplayName("Automatic 'unflattening'")]
        public void NameBasedUnflattening(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithNestedValue, ClassWithNested<ClassWithValue>>();
            var source = new ClassWithNestedValue { NestedValue = "ABC" };
            var result = adapter.Map<ClassWithNested<ClassWithValue>>(source);

            Assert.Equal(source.NestedValue, result.Nested.Value);
        }

        [Feature]
        [DisplayName("Method to property (same name)")]
        public void MethodToProperty(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithValueAsMethod, ClassWithValue>();
            var source = new ClassWithValueAsMethod("ABC");
            var result = adapter.Map<ClassWithValue>(source);

            Assert.Equal(source.Value(), result.Value);
        }

        [Feature]
        [DisplayName("Method to property (Get+name)")]
        public void GetMethodToProperty(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithGetValueMethod, ClassWithValue>();
            var source = new ClassWithGetValueMethod("ABC");
            var result = adapter.Map<ClassWithValue>(source);

            Assert.Equal(source.GetValue(), result.Value);
        }

        [Feature]
        [DisplayName("Property to method (Set+name)")]
        public void PropertyToSetMethod(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithValue, ClassWithSetValue>();
            var source = new ClassWithValue { Value = "ABC" };
            var result = adapter.Map<ClassWithSetValue>(source);

            Assert.Equal(source.Value, result.GetValue());
        }

        [Feature]
        [DisplayName("Non-static API")]
        public void NonStaticApi(IObjectMapperAdapter adapter) {
            Assert.True(adapter.MapperType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any());
        }
    }
}
