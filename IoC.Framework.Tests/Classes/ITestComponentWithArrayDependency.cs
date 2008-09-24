using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Research.IoC.Frameworks.Tests.Classes {
    public interface ITestComponentWithArrayDependency {
        ITestService[] Services { get; }
    }
}
