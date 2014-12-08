using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.Serializers.Adapters {
    public interface ISerializerAdapter : ILibrary {
        object DeserializeJsonOrXml(Type resultType, string json, string xml);
        string SerializeJsonOrXml(object value);
    }
}
