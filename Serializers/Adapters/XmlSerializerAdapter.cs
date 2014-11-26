using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace FeatureTests.On.Serializers.Adapters {
    public class XmlSerializerAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return new XmlSerializer(resultType).Deserialize(new StringReader(xml));
        }

        public override string SerializeJsonOrXml(object value) {
            var writer = new StringWriter();
            new XmlSerializer(value.GetType()).Serialize(writer, value);
            return writer.ToString();
        }

        public override string PackageId {
            get { return null; }
        }

        public override Assembly Assembly {
            get { return typeof(XmlSerializer).Assembly; }
        }
    }
}
