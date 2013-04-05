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
            return this.GetAdapters().Select(adapter => new FrameworkTestCommand(method, adapter));
        }

        private IEnumerable<IFrameworkAdapter> GetAdapters() {
            yield return new AutofacAdapter();
            yield return new CastleAdapter();
            yield return new LinFuAdapter();
            yield return new NinjectAdapter();
            yield return new StructureMapAdapter();
            yield return new SpringAdapter();
            yield return new UnityAdapter();
        }
    }
}
