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
        [DisplayName("Basic (Roundtrip)")]
        [Description("Uses whatever format serializer prefers.")]
        public void Roundtrip(IJsonSerializerAdapter adapter) {
            AssertForDeserializedRoundtrip(adapter, new Dictionary<string, object> { { "key", "value" } }, dictionary => {
                Assert.NotNull(dictionary);
                Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            });
        }

        [Feature]
        [DisplayName("Nested (Roundtrip)")]
        [Description("Uses whatever format serializer prefers.")]
        public void NestedRoundtrip(IJsonSerializerAdapter adapter) {
            AssertForDeserializedRoundtrip(
                adapter, new Dictionary<string, object> {
                    { "key", new Dictionary<string, object> { { "nested", "value" } } }
                },
                dictionary => {
                    Assert.NotNull(dictionary);
                    var nested = Assert.IsAssignableFrom<IDictionary<string, object>>(dictionary.GetValueOrDefault("key"));
                    Assert.Equal("value", nested.GetValueOrDefault("nested"));
                }
            );
        }

        [Feature]
        [DisplayName("Basic (Clean)")]
        public void Basic(IJsonSerializerAdapter adapter) {
            AssertForDeserialized(adapter, "{ \"key\": \"value\" }", dictionary => {
                Assert.NotNull(dictionary);
                Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            });
        }

        [Feature]
        [DisplayName("Nested (Clean)")]
        public void Nested(IJsonSerializerAdapter adapter) {
            AssertForDeserialized(adapter, "{ \"key\": { \"nested\": \"value\" } }", dictionary => {
                Assert.NotNull(dictionary);
                var nested = Assert.IsAssignableFrom<IDictionary<string, object>>(dictionary.GetValueOrDefault("key"));
                Assert.Equal("value", nested.GetValueOrDefault("nested"));
            });
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertForDeserialized(IJsonSerializerAdapter adapter, string json, Action<IDictionary<string, object>> assert) {
            var deserialized = adapter.Deserialize<IDictionary<string, object>>(json);
            assert(deserialized);
        }

        private void AssertForDeserializedRoundtrip(IJsonSerializerAdapter adapter, IDictionary<string, object> value, Action<IDictionary<string, object>> assert) {
            var serialized = adapter.Serialize(value);
            var deserialized = adapter.Deserialize<IDictionary<string, object>>(serialized);
            assert(deserialized);
        }
    }
}
