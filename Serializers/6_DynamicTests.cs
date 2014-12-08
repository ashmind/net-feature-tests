using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FeatureTests.On.Serializers.Adapters;
using FeatureTests.On.Serializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.Serializers {
    [DisplayOrder(6)]
    [DisplayName("Dynamic")]
    [Description(@"
        Sometimes it is inconvenient or impossible to define schema as a static type in advance.
        This leaves two options -- dictionaries and dynamic types. These tests look at dynamic support.
    ")]
    public class DynamicTests {
        [Feature]
        [DisplayName("dynamic => int")]
        public void CastToInt32(ISerializerAdapter adapter) {
            AssertForDeserialized(adapter, "5", "5", d => Assert.Equal(5, (int)d));
        }

        [Feature]
        [DisplayName("dynamic => string")]
        public void CastToString(ISerializerAdapter adapter) {
            AssertForDeserialized(adapter, "\"x\"", "x", d => Assert.Equal("x", (string)d));
        }

        [Feature]
        [DisplayName("dynamic property access")]
        public void PropertyAccess(ISerializerAdapter adapter) {
            AssertForDeserialized(
                adapter,
                "{ \"property\": \"value\" }",
                "<property>value</property>",
                d => Assert.Equal("value", (string)d.property)
            );
        }

        [Feature]
        [DisplayName("dynamic cast to type")]
        public void CastToType(ISerializerAdapter adapter) {
            AssertForDeserialized(
                adapter,
                "{ \"Value\": \"test\" }",
                "<Value>test</Value>",
                d => Assert.Equal("test", ((ClassWithSingleProperty<string>)d).Value)
            );
        }
        
        // ReSharper disable once UnusedParameter.Local
        private void AssertForDeserialized(ISerializerAdapter adapter, string valueInJson, string valueInXml, Action<dynamic> assert) {
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleProperty<dynamic>>(
                "{ \"Value\": " + valueInJson + " }",
                "<Root><Value>" + valueInXml + "</Value></Root>"
            );
            if (deserialized.Value != null) {
                Debug.WriteLine("Actual type of deserialized value: " + ((object)deserialized.Value).GetType());
            }
            else {
                Debug.WriteLine("Deserialized value is null.");
            }
            assert(deserialized.Value);
        }
    }
}
