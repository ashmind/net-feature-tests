using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FeatureTests.On.Serializers.Adapters;
using FeatureTests.On.Serializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.Serializers {
    [DisplayOrder(2)]
    [DisplayName("Collections")]
    public class CollectionTests {
        private const string ABListJson = "[\"A\", \"B\"]";

        [Feature]
        [DisplayName("string[]")]
        public void ArrayOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, new[] { "A", "B" });
        }
        
        [Feature]
        [DisplayName("object[]")]
        [Description(@"This test verifies that a mixed-type array can be deserialized.")]
        public void ArrayOfObject(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, "[\"A\", 5]", new object[] { "A", 5 });
        }

        [Feature]
        [DisplayName("List")]
        public void ListOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, new List<string> { "A", "B" });
        }

        [Feature]
        [DisplayName("IList")]
        public void IListOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, (IList<string>)new List<string> { "A", "B" });
        }

        [Feature]
        [DisplayName("HashSet")]
        public void HashSetOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, new HashSet<string> { "A", "B" });
        }

        [Feature]
        [DisplayName("ISet")]
        public void ISetOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, (ISet<string>)new HashSet<string> { "A", "B" });
        }

        [Feature]
        [DisplayName("IReadOnlyList")]
        public void IReadOnlyListOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, (IReadOnlyList<string>)new[] { "A", "B" });
        }

        [Feature]
        [DisplayName("IReadOnlyCollection")]
        public void IReadOnlyCollectionOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, (IReadOnlyCollection<string>)new[] { "A", "B" });
        }

        [Feature]
        [DisplayName("IEnumerable")]
        public void IEnumerableOfString(ISerializerAdapter adapter) {
            AssertCanBeDeserialized(adapter, ABListJson, (IEnumerable<string>)new[] { "A", "B" });
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertCanBeDeserialized<TCollection>(ISerializerAdapter adapter, string valueInJson, TCollection expected) 
            where TCollection : IEnumerable
        {
            var serialized = adapter.SerializeJsonOrXml(new RootClassWithSingleProperty<TCollection> {
                Value = expected
            });
            Debug.WriteLine("Expected by serializer:\r\n" + serialized);
            // since there is no standard way to represent lists in XML, it just tests roundtrip
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleProperty<TCollection>>(
                "{ \"Value\": " + valueInJson + " }",
                serialized
            );
            Assert.Equal(
                expected.Cast<object>().ToArray(),
                deserialized.Value.Cast<object>().ToArray()
            );
        }
    }
}
