using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class ServiceStackJsonSerializerAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return new JsonStringSerializer().DeserializeFromString(json, resultType);
        }

        public override string Serialize(object value) {
            return new JsonStringSerializer().SerializeToString(value);
        }

        public override string Name => "ServiceStack JSON";
        public override Assembly Assembly => typeof(JsonStringSerializer).Assembly;
    }
}
