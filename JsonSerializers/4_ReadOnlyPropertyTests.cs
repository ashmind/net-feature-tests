using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FeatureTests.On.JsonSerializers.Adapters;
using FeatureTests.On.JsonSerializers.TestTypes;
using FeatureTests.Shared;
using Xunit;

namespace FeatureTests.On.JsonSerializers {
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
        public void ICollection(IJsonSerializerAdapter adapter) {
            AssertCanBeRoundtripped<ICollection<string>, Collection<string>>(
                adapter, new Collection<string> { "A", "B" }
            );
        }

        [Feature]
        [DisplayName("ISet<T> property")]
        public void ISet(IJsonSerializerAdapter adapter) {
            AssertCanBeRoundtripped<ISet<string>, HashSet<string>>(
                adapter, new HashSet<string> { "A", "B" }
            );
        }

        [Feature]
        [DisplayName("Mutable class property")]
        public void MutableClass(IJsonSerializerAdapter adapter) {
            AssertCanBeRoundtripped<ClassWithSingleProperty<string>, ClassWithSingleProperty<string>>(
                adapter, new ClassWithSingleProperty<string> { Value = "A" }
            );
        }
        
        // ReSharper disable once UnusedParameter.Local
        private void AssertCanBeRoundtripped<TPropertyType, TActualType>(IJsonSerializerAdapter adapter, TActualType value)
            where TActualType : TPropertyType, new()
        {
            var serialized = adapter.Serialize(new RootClassWithSingleReadOnlyProperty<TPropertyType, TActualType>(value));
            Debug.WriteLine("Serialized:\r\n" + serialized);
            var deserialized = adapter.Deserialize<RootClassWithSingleReadOnlyProperty<TPropertyType, TActualType>>(serialized);

            Assert.Equal(value, deserialized.Value);
        }
    }
}
