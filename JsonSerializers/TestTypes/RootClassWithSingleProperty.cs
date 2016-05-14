using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FeatureTests.On.JsonSerializers.TestTypes {
    [DataContract(Name = "Root", Namespace = "")]
    [XmlRoot("Root")]
    [KnownType(typeof(DateTimeOffset))]
    public class RootClassWithSingleProperty<T> {
        [DataMember]
        public T Value { get; set; }
    }
}
