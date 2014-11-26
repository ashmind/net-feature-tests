using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AshMind.Extensions;
using FeatureTests.On.Serializers.Adapters;
using FeatureTests.On.Serializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.Serializers {
    [DisplayOrder(3)]
    [DisplayName("Dictionaries")]
    [Description(@"
        Sometimes it is inconvenient or impossible to define schema as a static type in advance.
        This leaves two options -- dictionaries and dynamic types. These tests look at dictionary support.

        There is no clear standard for dictionaries in XML, so XML tests might be flawed.
    ")]
    public class DictionaryTests {
        [Feature]
        [DisplayName("Basic (Roundtrip)")]
        [Description("Uses whatever format serializer prefers.")]
        public void Roundtrip(ISerializerAdapter adapter) {
            AssertForDeserializedRoundtrip(adapter, new Dictionary<string, object> { { "key", "value" } }, dictionary => {
                Assert.NotNull(dictionary);
                Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            });
        }

        [Feature]
        [DisplayName("Nested (Roundtrip)")]
        [Description("Uses whatever format serializer prefers.")]
        public void NestedRoundtrip(ISerializerAdapter adapter) {
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
        public void Basic(ISerializerAdapter adapter) {
            AssertForDeserialized(adapter, "{ \"key\": \"value\" }", "<key>value</key>", dictionary => {
                Assert.NotNull(dictionary);
                Assert.Equal("value", dictionary.GetValueOrDefault("key"));
            });
        }

        [Feature]
        [DisplayName("Nested (Clean)")]
        public void Nested(ISerializerAdapter adapter) {
            AssertForDeserialized(adapter, "{ \"key\": { \"nested\": \"value\" } }", "<key><nested>value</nested></key>", dictionary => {
                Assert.NotNull(dictionary);
                var nested = Assert.IsAssignableFrom<IDictionary<string, object>>(dictionary.GetValueOrDefault("key"));
                Assert.Equal("value", nested.GetValueOrDefault("nested"));
            });
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertForDeserialized(ISerializerAdapter adapter, string valueInJson, string valueInXml, Action<IDictionary<string, object>> assert) {
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleProperty<IDictionary<string, object>>>(
                "{ \"Value\": " + valueInJson + " }",
                "<Root><Value>" + valueInXml + "</Value></Root>"
            );
            assert(deserialized.Value);
        }

        private void AssertForDeserializedRoundtrip(ISerializerAdapter adapter, IDictionary<string, object> value, Action<IDictionary<string, object>> assert) {
            var serialized = adapter.SerializeJsonOrXml(new RootClassWithSingleProperty<IDictionary<string, object>> { Value = value });
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleProperty<IDictionary<string, object>>>(serialized, serialized);
            assert(deserialized.Value);
        }
    }
}
