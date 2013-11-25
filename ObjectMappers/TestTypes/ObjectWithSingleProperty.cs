using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class ObjectWithSingleProperty<T> {
        public T Value { get; set; }
    }
}
