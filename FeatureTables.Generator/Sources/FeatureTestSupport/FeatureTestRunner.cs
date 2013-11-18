using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AshMind.Extensions;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.XunitSupport;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    public class FeatureTestRunner {
        // potentially I could have used Xunit runners, but they are a bit annoying to get through NuGet
        public IReadOnlyCollection<FeatureTestRun> RunAllTests(Assembly assembly) {
            var all = assembly.GetTypes()
                              .SelectMany(t => t.GetMethods())
                              .Where(m => m.IsDefined<FeatureAttribute>(false))
                              .ToArray();

            var runs = new List<FeatureTestRun>();

            foreach (var test in all) {
                foreach (var frameworkType in Frameworks.TypeList()) {
                    RunTestWithDependencyHandling(runs, test, frameworkType, all);
                }
            }

            return runs;
        }

        private FeatureTestRun RunTestWithDependencyHandling(ICollection<FeatureTestRun> runs, MethodInfo test, Type frameworkType, MethodInfo[] allTests) {
            var run = runs.SingleOrDefault(r => r.Method == test && r.FrameworkType == frameworkType);
            if (run != null) // already run as a dependency?
                return run;
            
            var dependencies = test.GetCustomAttributes<DependsOnFeatureAttribute>();
            var requiredRuns = new List<FeatureTestRun>();
            foreach (var dependency in dependencies) {
                var dependencyDeclaringType = dependency.DeclaringType ?? test.DeclaringType;
                var requiredTest = allTests.SingleOrDefault(t => t.Name == dependency.MethodName && t.DeclaringType == dependencyDeclaringType);
                if (requiredTest == null)
                    throw new InvalidOperationException(string.Format("Could not find test '{0}' in type '{1}' (referenced by [FeatureDependsOn]).", dependency.MethodName, dependencyDeclaringType.Name));

                var requiredRun = this.RunTestWithDependencyHandling(runs, requiredTest, frameworkType, allTests);
                requiredRuns.Add(requiredRun);
            }

            run = new FeatureTestRun(test, frameworkType, RunTestAsync(test, frameworkType, requiredRuns));
            runs.Add(run);

            return run;
        }

        private async Task<FeatureTestResult> RunTestAsync(MethodInfo test, Type frameworkType, IEnumerable<FeatureTestRun> dependencies) {
            var specialCase = GetSpecialCase(test, frameworkType)
                           ?? GetSpecialCase(test.DeclaringType, frameworkType);

            if (specialCase != null && specialCase.Skip)
                return new FeatureTestResult(FeatureTestResultKind.SkippedDueToSpecialCase, specialCase.Comment);
            
            foreach (var dependency in dependencies) {
                var result = await dependency.Task;
                if (result.Kind != FeatureTestResultKind.Success) {
                    var className = AttributeHelper.GetDisplayName(dependency.Method.DeclaringType);
                    var testName = AttributeHelper.GetDisplayName(dependency.Method);

                    var skippedComment = string.Format("Skipped as {0} ({1}) is not supported by this framework.", testName, className);
                    return new FeatureTestResult(FeatureTestResultKind.SkippedDueToDependency, skippedComment);
                }
            }

            var instance = Activator.CreateInstance(test.DeclaringType);
            try {
                await Task.Run(() => test.Invoke(instance, new object[] { Frameworks.Get(frameworkType) }));
            }
            catch (Exception ex) {
                return new FeatureTestResult(FeatureTestResultKind.Failure, exception: ToUsefulException(ex));
            }

            var comment = specialCase != null ? specialCase.Comment : null;
            return new FeatureTestResult(FeatureTestResultKind.Success, comment);
        }

        private SpecialCaseAttribute GetSpecialCase(ICustomAttributeProvider member, Type frameworkType) {
            // it is definitely slow to call reflection each time, however it does not matter
            // for the test run
            return member.GetCustomAttributes<SpecialCaseAttribute>()
                         .SingleOrDefault(c => c.FrameworkType == frameworkType);
        }

        private static Exception ToUsefulException(Exception exception) {
            var invocationException = exception as TargetInvocationException;
            if (invocationException != null)
                return ToUsefulException(invocationException.InnerException);

            return exception;
        }
    }
}
