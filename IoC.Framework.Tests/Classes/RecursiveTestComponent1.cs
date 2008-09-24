using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public class RecursiveTestComponent1 {
        public RecursiveTestComponent1(RecursiveTestComponent2 dependency) {
        }
    }
}
