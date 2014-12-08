using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace FeatureTests.On.Serializers.Adapters {
    public class JavaScriptSerializerAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return new JavaScriptSerializer().Deserialize(json, resultType);
        }

        public override string SerializeJsonOrXml(object value) {
            return new JavaScriptSerializer().Serialize(value);
        }

        public override string PackageId {
            get { return null; }
        }

        public override Assembly Assembly {
            get { return typeof(JavaScriptSerializer).Assembly; }
        }
    }
}
