using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace FeatureTests.On.JsonSerializers.Adapters {
    public class DataContractJsonSerializerAdapter : JsonSerializerAdapterBase {
        public override object Deserialize(Type resultType, string json) {
            return new DataContractJsonSerializer(resultType).ReadObject(
                new MemoryStream(Encoding.UTF8.GetBytes(json))
            );
        }

        public override string Serialize(object value) {
            var stream = new MemoryStream();
            new DataContractJsonSerializer(value.GetType()).WriteObject(stream, value);
            stream.Seek(0, SeekOrigin.Begin);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public override string PackageId => null;
        public override Assembly Assembly => typeof (DataContractSerializer).Assembly;
    }
}
