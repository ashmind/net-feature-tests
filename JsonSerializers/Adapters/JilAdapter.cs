using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jil;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class JilAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return JSON.Deserialize(json, resultType, Options.ISO8601);
        }

        public override string Serialize(object value) {
            return JSON.Serialize(value, Options.ISO8601);
        }

        public override Assembly Assembly => typeof(JSON).Assembly;
    }
}
