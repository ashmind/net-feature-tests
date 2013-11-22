using System;
using System.Reflection;
using Xunit.Sdk;

namespace FeatureTests.Shared.InfrastructureSupport {
    public class FeatureTestCommand : TestCommand {
        private readonly ILibrary adapter;

        public FeatureTestCommand(IMethodInfo method, ILibrary adapter)
            : base(method, adapter.Name, MethodUtility.GetTimeoutParameter(method))
        {
            this.adapter = adapter;
        }

        public override MethodResult Execute(object testClass) {
            try {
                var parameters = this.testMethod.MethodInfo.GetParameters();
                if (parameters.Length != 1)
                    throw new InvalidOperationException("Feature test method must have a single parameter.");

                this.testMethod.Invoke(testClass, new[] { this.adapter });
            }
            catch (TargetInvocationException ex) {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}