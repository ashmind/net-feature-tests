using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.Serializers.Adapters;
using FeatureTests.On.Serializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.Serializers {
    [DisplayOrder(1)]
    [DisplayName("Simple types")]
    [Description("Tests on simple types. Since primitives are not valid in top level xml, this uses a class with a single property.")]
    public class SimpleTypeTests {
        [Feature]
        [DisplayName("string")]
        public void String(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, "\"ABC\"", "ABC", "ABC");
        }

        [Feature]
        [DisplayName("int")]
        public void Int32(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, "5", "5", 5);
        }

        [Feature]
        [DisplayName("DateTime")]
        public void DateTime(ISerializerAdapter adapter) {
            var date = new DateTime(2014, 08, 02, 12, 34, 56);
            AssertCanBeDeserialized(adapter, "\"" + date.ToString("O") + "\"", date.ToString("O"), date);
        }

        [Feature]
        [DisplayName("DateTimeOffset")]
        public void DateTimeOffset(ISerializerAdapter adapter) {
            var date = new DateTimeOffset(2014, 08, 02, 12, 34, 56, TimeSpan.FromHours(3));
            AssertCanBeDeserialized(adapter, "\"" + date.ToString("O") + "\"", date.ToString("O"), date);
        }

        [Feature]
        [DisplayName("Uri")]
        public void Uri(ISerializerAdapter adapter) {
            var uri = new Uri("http://google.com");
            AssertCanBeDeserialized(adapter, "\"" + uri + "\"", uri.ToString(), uri);
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertCanBeDeserialized<T>(ISerializerAdapter adapter, string valueInJson, string valueInXml, T expected) {
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleProperty<T>>(
                "{ \"Value\": " + valueInJson + " }",
                "<Root><Value>" + valueInXml + "</Value></Root>"
            );
            Assert.Equal(expected, deserialized.Value);
        }
    }
}
