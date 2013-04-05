using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjection.FeatureTests.Adapters;
using Xunit;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests.XunitSupport {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ForEachFrameworkAttribute : FactAttribute {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method) {
            return Frameworks.List().Select(adapter => new FrameworkTestCommand(method, adapter));
        }
    }
}
