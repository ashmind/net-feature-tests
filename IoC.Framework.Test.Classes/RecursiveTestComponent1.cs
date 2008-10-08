using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Test.Classes {
    public class RecursiveTestComponent1 {
        public RecursiveTestComponent1(RecursiveTestComponent2 dependency) {
        }
    }
}
