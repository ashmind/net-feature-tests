using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests.XunitSupport {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FeatureAttribute : FactAttribute {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method) {
            return Frameworks.List().Select(adapter => new FeatureTestCommand(method, adapter));
        }
    }
}
