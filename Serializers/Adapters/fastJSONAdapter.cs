using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fastJSON;

namespace FeatureTests.On.Serializers.Adapters {
    public class fastJSONAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return JSON.ToObject(json, resultType);
        }

        public override string SerializeJsonOrXml(object value) {
            return JSON.ToJSON(value);
        }

        public override Assembly Assembly {
            get { return typeof(JSON).Assembly; }
        }
    }
}
