using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YAXLib;

namespace FeatureTests.On.Serializers.Adapters {
    public class YAXLibAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return new YAXSerializer(resultType).Deserialize(xml);
        }

        public override string SerializeJsonOrXml(object value) {
            return new YAXSerializer(value.GetType()).Serialize(value);
        }

        public override Assembly Assembly {
            get { return typeof(YAXSerializer).Assembly; }
        }
    }
}
