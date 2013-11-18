using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    public class FeatureTestRun {
        public MethodInfo Method            { get; private set; }
        public Type FrameworkType           { get; private set; }
        public Task<FeatureTestResult> Task { get; private set; }

        public FeatureTestRun(MethodInfo method, Type frameworkType, Task<FeatureTestResult> task) {
            this.Method = method;
            this.FrameworkType = frameworkType;
            this.Task = task;
        }
    }
}