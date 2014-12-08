using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FeatureTests.On.Serializers.TestTypes {
    [DataContract(Name = "Root", Namespace = "")]
    [XmlRoot("Root")]
    public class RootClassWithCustomConstructor {
        public RootClassWithCustomConstructor(string value1, string value2) {
            Value1 = value1;
            Value2 = value2;
            ConstructorCalled = true;
        }

        // just to check if anyone uses GetUninitializedObject
        internal bool ConstructorCalled { get; private set; }
        [DataMember] public string Value1 { get; set; }
        [DataMember] public string Value2 { get; set; }       
        
        // for asserts
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals(obj as RootClassWithCustomConstructor);
        }

        public bool Equals(RootClassWithCustomConstructor other) {
            return AsAnonymous().Equals(other.AsAnonymous());
        }

        public override int GetHashCode() {
            return AsAnonymous().GetHashCode();
        }

        public override string ToString() {
            return AsAnonymous().ToString();
        }

        private object AsAnonymous() {
            return new { Value1, Value2 };
        }
    }
}
