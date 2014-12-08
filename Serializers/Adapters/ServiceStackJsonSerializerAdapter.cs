using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;

namespace FeatureTests.On.Serializers.Adapters {
    public class ServiceStackJsonSerializerAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return new JsonStringSerializer().DeserializeFromString(json, resultType);
        }

        public override string SerializeJsonOrXml(object value) {
            return new JsonStringSerializer().SerializeToString(value);
        }

        public override string Name {
            get { return "ServiceStack JSON"; }
        }

        public override Assembly Assembly {
            get { return typeof(JsonStringSerializer).Assembly; }
        }
    }
}
