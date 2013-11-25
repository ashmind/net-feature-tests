using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTests.On.ObjectMappers.TestTypes {
    public class TargetWithSingleProperty<T> {
        public T Value { get; set; }
    }
}
