using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace FeatureTests.On.Serializers.Adapters {
    public class DataContractSerializerAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return new DataContractSerializer(resultType).ReadObject(XmlReader.Create(new StringReader(xml)));
        }

        public override string SerializeJsonOrXml(object value) {
            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter);
            new DataContractSerializer(value.GetType())
                .WriteObject(xmlWriter, value);
            xmlWriter.Flush();

            return stringWriter.ToString();
        }

        public override string PackageId {
            get { return null; }
        }

        public override Assembly Assembly {
            get { return typeof (DataContractSerializer).Assembly; }
        }
    }
}
