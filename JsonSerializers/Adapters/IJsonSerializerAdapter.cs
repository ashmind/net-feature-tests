using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public interface IJsonSerializerAdapter : ILibrary {
        object Deserialize(Type resultType, string json);
        string Serialize(object value);
    }
}
