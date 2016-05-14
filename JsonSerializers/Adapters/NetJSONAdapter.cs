using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FeatureTests.On.JsonSerializers.Adapters {
    using NetJSON = NetJSON.NetJSON;

    public class NETJsonAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return NetJSON.Deserialize(resultType, json);
        }

        public override string Serialize(object value) {
            return NetJSON.Serialize(value);
        }

        public override Assembly Assembly => typeof(NetJSON).Assembly;
    }
}
