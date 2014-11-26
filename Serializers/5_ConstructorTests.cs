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
    [DisplayOrder(5)]
    [DisplayName("Constructors")]
    public class ConstructorTests {
        [Feature]
        [DisplayName("Custom constructor: deserialized")]
        public void CustomConstructorDeserialized(ISerializerAdapter adapter) {
            AssertCanBeRoundtripped(adapter, new RootClassWithCustomConstructor("A", "B"));
        }

        [Feature]
        [DisplayName("Custom constructor: actually called")]
        [Description(@"
            Some serializers might be using [FormatterServices.GetUninitializedObject](http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatterservices.getuninitializedobject%28v=vs.110%29.aspx).
            While it is a clever workaround, it wouldn't work with truly immutable class, and it would miss any constructor-level validation.
        ")]
        public void CustomConstructorCalled(ISerializerAdapter adapter) {
            AssertCanBeRoundtripped(
                adapter, new RootClassWithCustomConstructor("A", "B"),
                deserialized => Assert.True(deserialized.ConstructorCalled)
            );
        }
 
        // ReSharper disable once UnusedParameter.Local
        private void AssertCanBeRoundtripped<T>(ISerializerAdapter adapter, T instance, Action<T> assertExtra = null) {
            var serialized = adapter.SerializeJsonOrXml(instance);
            Debug.WriteLine("Serialized: " + serialized);
            var deserialized = adapter.DeserializeJsonOrXml<T>(serialized, serialized);

            Assert.Equal(instance, deserialized);
            if (assertExtra != null)
                assertExtra(deserialized);
        }
    }
}
