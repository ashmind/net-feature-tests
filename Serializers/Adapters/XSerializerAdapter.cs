using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeatureTests.Shared.GenericApiSupport;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;
using XSerializer;

namespace FeatureTests.On.Serializers.Adapters {
    public class XSerializerAdapter : SerializerAdapterBase {
        public override object DeserializeJsonOrXml(Type resultType, string json, string xml) {
            return GenericHelper.RewriteAndInvoke(
                () => new XmlSerializer<X1>().Deserialize(xml), resultType
            );
        }

        public override string SerializeJsonOrXml(object value) {
            return (string)GenericHelper.RewriteAndInvoke(
                () => new XmlSerializer<X1>().Serialize((X1)value), value.GetType()
            );
        }

        public override Assembly Assembly {
            get { return typeof(XmlSerializer).Assembly; }
        }
    }
}
