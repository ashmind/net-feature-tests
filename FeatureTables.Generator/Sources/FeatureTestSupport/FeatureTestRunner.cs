using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.XunitSupport;

namespace DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport {
    public class FeatureTestRunner {
        // potentially I could have used Xunit runners, but they are a bit annoying to get through NuGet
        public IEnumerable<FeatureTestRun> RunAllTests(Assembly assembly) {
            var all = assembly.GetTypes()
                              .SelectMany(t => t.GetMethods())
                              .Where(m => m.IsDefined<FeatureAttribute>(false))
                              .ToArray();

            var runs = new List<FeatureTestRun>();
            var frameworkTypes = Frameworks.List().Select(f => f.GetType()).ToArray();

            foreach (var test in all) {
                foreach (var frameworkType in frameworkTypes) {
                    // a bit stupid, but framework instances are not reusable between tests, and dependency aware 
                    // run may need framework for each encountered dependency
                    Func<IFrameworkAdapter> newFramework = () => Frameworks.List().First(f => f.GetType() == frameworkType);
                    RunTestWithDependencyHandling(runs, test, newFramework, all);
                }
            }

            return runs;
        }

        private FeatureTestRun RunTestWithDependencyHandling(ICollection<FeatureTestRun> runs, MethodInfo test, Func<IFrameworkAdapter> newFramework, MethodInfo[] allTests) {
            var framework = newFramework();
            var run = runs.SingleOrDefault(r => r.Method == test && r.FrameworkType == framework.GetType());
            if (run != null) // already run as a dependency?
                return run;

            var dependencies = test.GetCustomAttributes<DependsOnFeatureAttribute>();
            foreach (var dependency in dependencies) {
                var dependencyDeclaringType = dependency.DeclaringType ?? test.DeclaringType;
                var requiredTest = allTests.SingleOrDefault(t => t.Name == dependency.MethodName && t.DeclaringType == dependencyDeclaringType);
                if (requiredTest == null)
                    throw new InvalidOperationException(string.Format("Could not find test '{0}' in type '{1}' (referenced by [FeatureDependsOn]).", dependency.MethodName, dependencyDeclaringType.Name));

                var requiredRun = this.RunTestWithDependencyHandling(runs, requiredTest, newFramework, allTests);
                if (requiredRun.Result != FeatureTestResult.Success) {
                    var className = DisplayNameHelper.GetDisplayName(requiredTest.DeclaringType);
                    var testName = DisplayNameHelper.GetDisplayName(requiredTest);

                    var comment = string.Format("Skipped as {0} ({1}) is not supported by this framework.", testName, className);
                    run = new FeatureTestRun(test, framework.GetType(), FeatureTestResult.Skipped, comment);
                    runs.Add(run);
                    return run;
                }
            }

            run = RunTest(test, framework);
            runs.Add(run);

            return run;
        }

        private FeatureTestRun RunTest(MethodInfo test, IFrameworkAdapter framework) {
            var frameworkType = framework.GetType();
            var specialCase = GetSpecialCase(test, frameworkType)
                           ?? GetSpecialCase(test.DeclaringType, frameworkType);
            if (specialCase != null && specialCase.Skip)
                return new FeatureTestRun(test, frameworkType, FeatureTestResult.Skipped, specialCase.Comment);

            var instance = Activator.CreateInstance(test.DeclaringType);
            try {
                test.Invoke(instance, new object[] {framework});
            }
            catch (Exception ex) {
                return new FeatureTestRun(test, frameworkType, FeatureTestResult.Failure, exception: ToUsefulException(ex));
            }

            var comment = specialCase != null ? specialCase.Comment : null;
            return new FeatureTestRun(test, frameworkType, FeatureTestResult.Success, comment);
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
