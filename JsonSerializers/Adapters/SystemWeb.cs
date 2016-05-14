using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class JavaScriptSerializerAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return new JavaScriptSerializer().Deserialize(json, resultType);
        }

        public override string Serialize(object value) {
            return new JavaScriptSerializer().Serialize(value);
        }

        public override string PackageId => null;
        public override Assembly Assembly => typeof(JavaScriptSerializer).Assembly;
    }
}
