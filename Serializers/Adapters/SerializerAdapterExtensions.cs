using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.Serializers.Adapters {
    public static class SerializerAdapterExtensions {
        public static T DeserializeJsonOrXml<T>(this ISerializerAdapter adapter, string json, string xml) {
            return (T)adapter.DeserializeJsonOrXml(typeof(T), json, xml);
        }
    }
}
