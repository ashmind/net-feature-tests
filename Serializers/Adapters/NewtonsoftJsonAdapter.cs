using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FeatureTests.On.Serializers.Adapters {
    public class NewtonsoftJsonAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return JsonConvert.DeserializeObject(json, resultType);
        }

        public override string SerializeJsonOrXml(object value) {
            return JsonConvert.SerializeObject(value);
        }

        public override string Name {
            get { return "Newtonsoft.Json"; }
        }

        public override System.Reflection.Assembly Assembly {
            get { return typeof(JsonSerializer).Assembly; }
        }
    }
}
