using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fastJSON;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class fastJSONAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return JSON.ToObject(json, resultType);
        }

        public override string Serialize(object value) {
            return JSON.ToJSON(value);
        }

        public override Assembly Assembly => typeof(JSON).Assembly;
    }
}
