using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureTests.Shared.ResultData {
    public class Feature {
        public Feature(string name) : this(new object(), name) {
        }

        public Feature(object key, string name) {
            this.Key = key;
            this.Name = name;
        }

        public object Key { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
    }
}
