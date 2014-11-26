using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FeatureTests.On.Serializers.TestTypes {
    [DataContract(Name = "Root", Namespace = "")]
    [XmlRoot("Root")]
    [KnownType(typeof(DateTimeOffset))]
    public class RootClassWithSingleReadOnlyProperty<TPropertyType, TFieldType>
        where TFieldType : TPropertyType, new()
    {
        private readonly TFieldType value;

        internal RootClassWithSingleReadOnlyProperty(TFieldType value) {
            this.value = value;
        }

        public RootClassWithSingleReadOnlyProperty()
            : this(new TFieldType()) {
        }
            
        [DataMember]
        public TPropertyType Value {
            get { return this.value; }
        }
    }
}
