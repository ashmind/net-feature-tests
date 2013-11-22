using System;

namespace FeatureTests.Runner.Sources.FeatureTestSupport {
    public class FeatureTestResult {
        public FeatureTestResultKind Kind { get; private set; }
        public string Message             { get; private set; }
        public Exception Exception        { get; private set; }

        public FeatureTestResult(FeatureTestResultKind kind, string message = null, Exception exception = null) {
            this.Kind = kind;
            this.Message = message;
            this.Exception = exception;
        }
    }
}