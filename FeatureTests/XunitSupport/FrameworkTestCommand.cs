using System;
using System.Reflection;
using DependencyInjection.FeatureTests.Adapters;
using Xunit.Sdk;

namespace DependencyInjection.FeatureTests.XunitSupport {
    public class FrameworkTestCommand : TestCommand {
        private readonly IFrameworkAdapter adapter;

        public FrameworkTestCommand(IMethodInfo method, IFrameworkAdapter adapter)
            : base(method, adapter.FrameworkName, MethodUtility.GetTimeoutParameter(method))
        {
            this.adapter = adapter;
        }

        public override MethodResult Execute(object testClass) {
            try {
                var parameters = this.testMethod.MethodInfo.GetParameters();
                if (parameters.Length != 1)
                    throw new InvalidOperationException("ForEachFramework test method must have a single parameter.");

                this.testMethod.Invoke(testClass, new[] { adapter });
            }
            catch (TargetInvocationException ex) {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}