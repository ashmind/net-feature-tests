using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(7)]
    [DisplayName("Dynamic")]
    [Description("_TODO_")]
    public class DynamicTests {
        [Feature]
        [DisplayName("ExpandoObject to normal class")]
        public void ExpandoObjectToClass(IObjectMapperAdapter mapper) {
            mapper.CreateMap<dynamic, Wrapper<string>>();

            dynamic source = new ExpandoObject();
            source.Value = "ABC";
            
            var result = mapper.Map<Wrapper<string>>((object)source);

            Assert.NotNull(result);
            Assert.Equal("ABC", result.Value);
        }
    }
}