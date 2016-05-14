using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FeatureTests.On.JsonSerializers.Adapters;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.JsonSerializers {
    [DisplayOrder(2)]
    [DisplayName("Collections")]
    public class CollectionTests {
        private const string ABListJson = "[\"A\", \"B\"]";

        [Feature]
        [DisplayName("string[]")]
        public void ArrayOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo(new[] { "A", "B" }, ABListJson, adapter);
        }
        
        [Feature]
        [DisplayName("object[]")]
        [Description(@"This test verifies that a mixed-type array can be deserialized.")]
        public void ArrayOfObject(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo(new object[] { "A", 5 }, "[\"A\", 5]", adapter);
        }

        [Feature]
        [DisplayName("List")]
        public void ListOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo(new List<string> { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("IList")]
        public void IListOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo((IList<string>)new List<string> { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("HashSet")]
        public void HashSetOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo(new HashSet<string> { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("ISet")]
        public void ISetOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo((ISet<string>)new HashSet<string> { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("IReadOnlyList")]
        public void IReadOnlyListOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo((IReadOnlyList<string>)new[] { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("IReadOnlyCollection")]
        public void IReadOnlyCollectionOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo((IReadOnlyCollection<string>)new[] { "A", "B" }, ABListJson, adapter);
        }

        [Feature]
        [DisplayName("IEnumerable")]
        public void IEnumerableOfString(IJsonSerializerAdapter adapter) {
            AssertDeserializesTo((IEnumerable<string>)new[] { "A", "B" }, ABListJson, adapter);
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertDeserializesTo<TCollection>(TCollection expected, string json, IJsonSerializerAdapter adapter) 
            where TCollection : IEnumerable
        {
            var deserialized = adapter.Deserialize<TCollection>(json);
            Assert.Equal(
                expected.Cast<object>().ToArray(),
                deserialized.Cast<object>().ToArray()
            );
        }
    }
}
