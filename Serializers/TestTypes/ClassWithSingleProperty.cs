using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FeatureTests.On.Serializers.TestTypes {
    [DataContract]
    public class ClassWithSingleProperty<T> {
        [DataMember]
        public T Value { get; set; }

        // for asserts
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals(obj as ClassWithSingleProperty<T>);
        }

        public bool Equals(ClassWithSingleProperty<T> other) {
            return Equals(Value, other.Value);
        }

        public override int GetHashCode() {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public override string ToString() {
            return new { Value }.ToString();
        }
    }
}
