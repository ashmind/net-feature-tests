using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FeatureTests.On.Serializers.TestTypes;
using NFormats.Xml;
using NFormats.Xml.Contracts;

namespace FeatureTests.On.Serializers.Adapters {
    public class NFormatsXmlAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return this.CreateSerializer().Deserialize(new StringReader(xml), resultType);
        }

        public override string SerializeJsonOrXml(object value) {
            var writer = new StringWriter();
            this.CreateSerializer().Serialize(writer, value);
            return writer.ToString();
        }

        private XmlSerializer CreateSerializer() {
            return new XmlSerializer(new XmlSerializationSettings {
                ContractResolver = new XmlContractResolver(false)
            });
        }

        public override string Name {
            get { return "NFormats.Xml"; }
        }

        public override Assembly Assembly {
            get { return typeof(XmlSerializer).Assembly; }
        }
    }
}
