using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.Serializers.Adapters {
    public abstract class SerializerAdapterBase : LibraryAdapterBase, ISerializerAdapter {
        public abstract object DeserializeJsonOrXml(Type resultType, string json, string xml);
        public abstract string SerializeJsonOrXml(object value);
    }
}
