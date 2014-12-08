using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FeatureTests.On.Serializers.Adapters;
using FeatureTests.On.Serializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.Serializers {
    [DisplayOrder(4)]
    [DisplayName("Read-only properties")]
    [Description(@"
        It is often a good idea to avoid public setters for certain properties.
        One classic example is collection properties, which are often recommended
        to be readonly to ensure the collection is always non-null.

        Since there are other ways to set those properties (e.g. Add for collections),
        there should be no need for bad design just to satisfy the serializer.
    ")]
    public class ReadOnlyPropertyTests {
        [Feature]
        [DisplayName("ICollection<T> property")]
        public void ICollection(ISerializerAdapter adapter) {
            AssertCanBeRoundtripped<ICollection<string>, Collection<string>>(
                adapter, new Collection<string> { "A", "B" }
            );
        }

        [Feature]
        [DisplayName("ISet<T> property")]
        public void ISet(ISerializerAdapter adapter) {
            AssertCanBeRoundtripped<ISet<string>, HashSet<string>>(
                adapter, new HashSet<string> { "A", "B" }
            );
        }

        [Feature]
        [DisplayName("Mutable class property")]
        public void MutableClass(ISerializerAdapter adapter) {
            AssertCanBeRoundtripped<ClassWithSingleProperty<string>, ClassWithSingleProperty<string>>(
                adapter, new ClassWithSingleProperty<string> { Value = "A" }
            );
        }
        
        // ReSharper disable once UnusedParameter.Local
        private void AssertCanBeRoundtripped<TPropertyType, TActualType>(ISerializerAdapter adapter, TActualType value)
            where TActualType : TPropertyType, new()
        {
            var serialized = adapter.SerializeJsonOrXml(new RootClassWithSingleReadOnlyProperty<TPropertyType, TActualType>(value));
            Debug.WriteLine("Serialized:\r\n" + serialized);
            var deserialized = adapter.DeserializeJsonOrXml<RootClassWithSingleReadOnlyProperty<TPropertyType, TActualType>>(serialized, serialized);

            Assert.Equal(value, deserialized.Value);
        }
    }
}
