using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FeatureTests.On.JsonSerializers.Adapters;
using FeatureTests.On.JsonSerializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.JsonSerializers {
    [DisplayOrder(6)]
    [DisplayName("Dynamic")]
    [Description(@"
        Sometimes it is inconvenient or impossible to define schema as a static type in advance.
        This leaves two options -- dictionaries and dynamic types. These tests look at dynamic support.
    ")]
    public class DynamicTests {
        [Feature]
        [DisplayName("dynamic => int")]
        public void CastToInt32(IJsonSerializerAdapter adapter) {
            var deserialized = adapter.Deserialize<dynamic>("5");
            Assert.Equal(5, (int)deserialized);
        }

        [Feature]
        [DisplayName("dynamic => string")]
        public void CastToString(IJsonSerializerAdapter adapter) {
            var deserialized = adapter.Deserialize<dynamic>("\"x\"");
            Assert.Equal("x", (string)deserialized);
        }

        [Feature]
        [DisplayName("dynamic property access")]
        public void PropertyAccess(IJsonSerializerAdapter adapter) {
            var deserialized = adapter.Deserialize<dynamic>("{ \"property\": \"value\" }");
            Assert.Equal("value", (string)deserialized.property);
        }

        [Feature]
        [DisplayName("dynamic cast to type")]
        public void CastToType(IJsonSerializerAdapter adapter) {
            var deserialized = adapter.Deserialize<dynamic>("{ \"Value\": \"test\" }");
            Assert.Equal("test", ((ClassWithSingleProperty<string>)deserialized).Value);
        }
    }
}
