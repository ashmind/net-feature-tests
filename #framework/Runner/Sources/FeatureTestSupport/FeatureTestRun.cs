using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FeatureTests.Runner.Sources.FeatureTestSupport {
    public class FeatureTestRun {
        public MethodInfo Method            { get; private set; }
        public Type AdapterType             { get; private set; }
        public Task<FeatureTestResult> Task { get; private set; }

        public FeatureTestRun(MethodInfo method, Type adapterType, Task<FeatureTestResult> task) {
            this.Method = method;
            this.AdapterType = adapterType;
            this.Task = task;
        }
    }
}