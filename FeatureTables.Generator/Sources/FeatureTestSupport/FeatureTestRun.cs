using System;
using System.Reflection;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    public class FeatureTestRun {
        public MethodInfo Method        { get; private set; }
        public Type FrameworkType       { get; private set; }
        public FeatureTestResult Result { get; private set; }
        public string Message           { get; private set; }
        public Exception Exception      { get; private set; }

        public FeatureTestRun(MethodInfo method, Type frameworkType, FeatureTestResult result, string message = null, Exception exception = null) {
            this.Method = method;
            this.FrameworkType = frameworkType;
            this.Result = result;
            this.Message = message;
            this.Exception = exception;
        }
    }
}