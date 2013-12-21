using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.ObjectMappers.Adapters;
using FeatureTests.On.ObjectMappers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.ObjectMappers {
    [DisplayOrder(8)]
    [DisplayName("Dictionaries")]
    [Description("_TODO_")]
    public class DictionaryTests {
        [Feature]
        [DisplayName("Property to dictionary entry")]
        public void PropertyToDictionaryEntry(IObjectMapperAdapter mapper) {
            mapper.CreateMap<Wrapper<string>, Dictionary<string, object>>();

            var source = new Wrapper<string> { Value = "ABC" };
            var result = mapper.Map<Dictionary<string, object>>(source);
            
            Assert.NotNull(result);
            Assert.Contains(new KeyValuePair<string, object>("Value", "ABC"), result);
        }

        [Feature]
        [DisplayName("Dictionary entry to property")]
        public void DictionaryEntryToProperty(IObjectMapperAdapter mapper) {
            mapper.CreateMap<Dictionary<string, object>, Wrapper<string>>();

            var source = new Dictionary<string, object> { { "Value", "ABC" } };
            var result = mapper.Map<Wrapper<string>>(source);

            Assert.NotNull(result);
            Assert.Equal("ABC", result.Value);
        }
    }
}