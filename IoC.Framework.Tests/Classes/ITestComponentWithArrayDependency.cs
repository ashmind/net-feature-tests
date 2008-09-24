using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Tests.Classes {
    public interface ITestComponentWithArrayDependency {
        ITestService[] Services { get; }
    }
}
