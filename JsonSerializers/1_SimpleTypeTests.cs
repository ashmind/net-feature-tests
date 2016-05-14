using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.JsonSerializers.Adapters;
using FeatureTests.On.JsonSerializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.JsonSerializers {
    [DisplayOrder(1)]
    [DisplayName("Simple types")]
    [Description("Tests on simple types -- top level primitive.")]
    public class SimpleTypeTests {
        private const string Iso8601WithTimeZoneFormat = @"yyyy-MM-ddTHH\:mm\:ss.fffffffK";

        [Feature]
        [DisplayName("string")]
        public void String(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo("ABC", "\"ABC\"", adapter);
        }

        [Feature]
        [DisplayName("int")]
        public void Int32(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo(5, "5", adapter);
        }

        [Feature]
        [DisplayName("DateTime")]
        public void DateTime(IJsonSerializerAdapter adapter) {
            var date = new DateTime(2014, 08, 02, 12, 34, 56, DateTimeKind.Utc);
            AssertDeserializesTo(date, "\"" + date.ToString(Iso8601WithTimeZoneFormat) + "\"", adapter);
        }

        [Feature]
        [DisplayName("DateTimeOffset")]
        public void DateTimeOffset(IJsonSerializerAdapter adapter) {
            var date = new DateTimeOffset(2014, 08, 02, 12, 34, 56, TimeSpan.FromHours(3));
            AssertDeserializesTo(date, "\"" + date.ToString(Iso8601WithTimeZoneFormat) + "\"", adapter);
        }

        [Feature]
        [DisplayName("Uri")]
        public void Uri(IJsonSerializerAdapter adapter) {
            var uri = new Uri("http://google.com");
            AssertDeserializesTo(uri, "\"" + uri + "\"", adapter);
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertDeserializesTo<T>(T expected, string json, IJsonSerializerAdapter adapter) {
            Assert.Equal(expected, adapter.Deserialize<T>(json));
        }
    }
}
