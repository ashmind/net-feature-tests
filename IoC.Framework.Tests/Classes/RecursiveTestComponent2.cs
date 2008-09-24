using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Classes {
    public class RecursiveTestComponent2 {
        public RecursiveTestComponent2(RecursiveTestComponent1 dependency) {
        }
    }
}
