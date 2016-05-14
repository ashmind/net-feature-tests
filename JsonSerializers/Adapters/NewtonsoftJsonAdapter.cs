using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class NewtonsoftJsonAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return JsonConvert.DeserializeObject(json, resultType);
        }

        public override string Serialize(object value) {
            return JsonConvert.SerializeObject(value);
        }

        public override string Name => "Newtonsoft.Json";
        public override Assembly Assembly => typeof(JsonSerializer).Assembly;
    }
}
