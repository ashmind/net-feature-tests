using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public static class SerializerAdapterExtensions {
        public static T Deserialize<T>(this IJsonSerializerAdapter adapter, string json) {
            return (T)adapter.Deserialize(typeof(T), json);
        }
    }
}
