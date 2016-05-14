using System;
using System.Collections.Generic;
using System.ComponentModel;
using AshMind.Extensions;
using FeatureTests.On.JsonSerializers.Adapters;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.JsonSerializers {
    [DisplayOrder(3)]
    [DisplayName("Dictionaries")]
    [Description(@"
        Sometimes it is inconvenient or impossible to define schema as a static type in advance.
        This leaves two options -- dictionaries and dynamic types. These tests look at dictionary support.
    ")]
    public class DictionaryTests {
        [Feature]
        [DisplayName("Basic")]
        public void Basic(IJsonSerializerAdapter adapter) {
            var dictionary = adapter.Deserialize<IDictionary<string, object>>("{ \"key\": \"value\" }");
            Assert.NotNull(dictionary);
            Assert.Equal("value", dictionary.GetValueOrDefault("key"));
        }

        [Feature]
        [DisplayName("Nested")]
        public void Nested(IJsonSerializerAdapter adapter) {
            var dictionary = adapter.Deserialize<IDictionary<string, object>>("{ \"key\": { \"nested\": \"value\" } }");
            Assert.NotNull(dictionary);
            var nested = Assert.IsAssignableFrom<IDictionary<string, object>>(dictionary.GetValueOrDefault("key"));
            Assert.Equal("value", nested.GetValueOrDefault("nested"));
        }
    }
}
