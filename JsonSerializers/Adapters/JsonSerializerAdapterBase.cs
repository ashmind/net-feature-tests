using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public abstract class JsonSerializerAdapterBase : LibraryAdapterBase, IJsonSerializerAdapter {
        public abstract object Deserialize(Type resultType, string json);
        public abstract string Serialize(object value);
    }
}
