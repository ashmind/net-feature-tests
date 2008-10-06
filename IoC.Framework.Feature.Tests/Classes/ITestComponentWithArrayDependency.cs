using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Framework.Feature.Tests.Classes {
    public interface ITestComponentWithArrayDependency {
        ITestService[] Services { get; }
    }
}
