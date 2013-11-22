using System;
using System.Collections.Generic;
using System.Linq;
using FeatureTests.Shared.InfrastructureSupport;
using Xunit;
using Xunit.Sdk;

namespace FeatureTests.Shared {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FeatureAttribute : FactAttribute {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method) {
            return LibraryProvider.GetAdapters(method.GetType().Assembly)
                                  .Select(adapter => new FeatureTestCommand(method, adapter));
        }
    }
}
