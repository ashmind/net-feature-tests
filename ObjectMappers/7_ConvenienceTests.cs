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
            adapter.CreateMap<ClassWithNested<ClassWithName>, ClassWithNestedName>();
            var source = new ClassWithNested<ClassWithName> { Nested = { Name = "ABC" } };
            var result = adapter.Map<ClassWithNestedName>(source);

            Assert.Equal(source.Nested.Name, result.NestedName);
        }

        [Feature]
        [DisplayName("Automatic 'unflattening'")]
        public void NameBasedUnflattening(IObjectMapperAdapter adapter) {
            adapter.CreateMap<ClassWithNestedName, ClassWithNested<ClassWithName>>();
            var source = new ClassWithNestedName { NestedName = "ABC" };
            var result = adapter.Map<ClassWithNested<ClassWithName>>(source);

            Assert.Equal(source.NestedName, result.Nested.Name);
        }

        [Feature]
        [DisplayName("Non-static API")]
        public void NonStaticApi(IObjectMapperAdapter adapter) {
            Assert.True(adapter.MapperType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any());
        }
    }
}
