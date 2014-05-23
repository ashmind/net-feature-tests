using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AshMind.Extensions;
using FeatureTests.Shared;
using FeatureTests.Shared.InfrastructureSupport;

namespace FeatureTests.Runner.Sources.FeatureTestSupport {
    public class FeatureTestRunner {
        // potentially I could have used Xunit runners, but they are a bit annoying to get through NuGet
        public IReadOnlyCollection<FeatureTestRun> RunAllTests(Assembly assembly) {
            var all = assembly.GetTypes()
                              .SelectMany(t => t.GetMethods())
                              .Where(m => m.IsDefined<FeatureAttribute>(false))
                              .ToArray();

            var runs = new List<FeatureTestRun>();

            foreach (var test in all) {
                foreach (var adapterType in LibraryProvider.GetAdapterTypes(assembly)) {
                    this.RunTestWithDependencyHandling(runs, test, adapterType, all);
                }
            }

            return runs;
        }

        private FeatureTestRun RunTestWithDependencyHandling(ICollection<FeatureTestRun> runs, MethodInfo test, Type adapterType, MethodInfo[] allTests) {
            var run = runs.SingleOrDefault(r => r.Method == test && r.AdapterType == adapterType);
            if (run != null) // already run as a dependency?
                return run;
            
            var dependencies = test.GetCustomAttributes<DependsOnFeatureAttribute>();
            var requiredRuns = new List<FeatureTestRun>();
            foreach (var dependency in dependencies) {
                var dependencyDeclaringType = dependency.DeclaringType ?? test.DeclaringType;
                var requiredTest = allTests.SingleOrDefault(t => t.Name == dependency.MethodName && t.DeclaringType == dependencyDeclaringType);
                if (requiredTest == null)
                    throw new InvalidOperationException(string.Format("Could not find test '{0}' in type '{1}' (referenced by [FeatureDependsOn]).", dependency.MethodName, dependencyDeclaringType.Name));

                var requiredRun = this.RunTestWithDependencyHandling(runs, requiredTest, adapterType, allTests);
                requiredRuns.Add(requiredRun);
            }

            run = new FeatureTestRun(test, adapterType, this.RunTestAsync(test, adapterType, requiredRuns));
            runs.Add(run);

            return run;
        }

        private async Task<FeatureTestResult> RunTestAsync(MethodInfo test, Type adapterType, IEnumerable<FeatureTestRun> dependencies) {
            var specialCase = this.GetSpecialCase(test, adapterType)
                           ?? this.GetSpecialCase(test.DeclaringType, adapterType);

            if (specialCase != null && specialCase.Skip)
                return new FeatureTestResult(FeatureTestResultKind.SkippedDueToSpecialCase, specialCase.Comment);
            
            foreach (var dependency in dependencies.OrderBy(d => d.Method.Name)) {
                var result = await dependency.Task;
                if (result.Kind != FeatureTestResultKind.Success) {
                    var className = FeatureTestAttributeHelper.GetDisplayName(dependency.Method.DeclaringType);
                    var testName = FeatureTestAttributeHelper.GetDisplayName(dependency.Method);

                    var skippedComment = string.Format("Skipped as {0} ({1}) is not supported by this library.", testName, className);
                    return new FeatureTestResult(FeatureTestResultKind.SkippedDueToDependency, skippedComment);
                }
            }

            var instance = Activator.CreateInstance(test.DeclaringType);
            using (instance as IDisposable) {
                try {
                    await Task.Run(() => test.Invoke(instance, new object[] {LibraryProvider.CreateAdapter(adapterType)}));
                }
                catch (Exception ex) {
                    var useful = ToUsefulException(ex);
                    if (useful is SkipException)
                        return new FeatureTestResult(FeatureTestResultKind.SkippedDueToSpecialCase, useful.Message);

                    return new FeatureTestResult(FeatureTestResultKind.Failure, exception: useful);
                }
            }

            var comment = specialCase != null ? specialCase.Comment : null;
            return new FeatureTestResult(FeatureTestResultKind.Success, comment);
        }

        private SpecialCaseAttribute GetSpecialCase(ICustomAttributeProvider member, Type adapterType) {
            // it is definitely slow to call reflection each time, however it does not matter
            // for the test run
            return member.GetCustomAttributes<SpecialCaseAttribute>()
                         .SingleOrDefault(c => c.AdapterType == adapterType);
        }

        private static Exception ToUsefulException(Exception exception) {
            var invocationException = exception as TargetInvocationException;
            if (invocationException != null)
                return ToUsefulException(invocationException.InnerException);

            return exception;
        }
    }
}
