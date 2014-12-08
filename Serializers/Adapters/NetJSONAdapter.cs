using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.On.Serializers.Adapters {
    using NetJSON = NetJSON.NetJSON;

    public class NETJsonAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return NetJSON.Deserialize(resultType, json);
        }

        public override string SerializeJsonOrXml(object value) {
            return NetJSON.Serialize(value);
        }

        public override System.Reflection.Assembly Assembly {
            get { return typeof(NetJSON).Assembly; }
        }
    }
}
