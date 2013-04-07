using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyInjection.TableGenerator.Data {
    public class Feature {
        public Feature(object key, string name) {
            this.Key = key;
            this.Name = name;
        }

        public object Key { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
    }
}
