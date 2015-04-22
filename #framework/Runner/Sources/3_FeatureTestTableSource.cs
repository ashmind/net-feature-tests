using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AshMind.Extensions;
using FeatureTests.Runner.Sources.FeatureTestSupport;
using FeatureTests.Shared.InfrastructureSupport;
using FeatureTests.Shared.ResultData;

namespace FeatureTests.Runner.Sources {
    public class FeatureTestTableSource : IFeatureTableSource {
        private readonly FeatureTestRunner runner;
        private readonly AttributeTextCleaner attributeCleaner;
        private readonly ExceptionNormalizer exceptionNormalizer;

        public FeatureTestTableSource(FeatureTestRunner runner, AttributeTextCleaner attributeCleaner, ExceptionNormalizer exceptionNormalizer) {
            this.runner = runner;
            this.attributeCleaner = attributeCleaner;
            this.exceptionNormalizer = exceptionNormalizer;
        }

        public IEnumerable<FeatureTable> GetTables(Assembly featureTestAssembly) {
            var testRuns = this.runner.RunAllTests(featureTestAssembly).ToDictionary(r => new { Test = r.Method, LibraryType = r.AdapterType });
            var testGroups = testRuns.Keys
                                     .Select(k => k.Test)
                                     .Distinct()
                                     .GroupBy(m => m.DeclaringType)
                                     .OrderBy(g => FeatureTestAttributeHelper.GetDisplayOrder(g.Key))
                                     .ToArray();

            var libraries = LibraryProvider.GetAdapters(featureTestAssembly).ToArray();
            foreach (var group in testGroups) {
                var features = group.ToDictionary(m => m, this.ConvertToFeature);
                var table = new FeatureTable(FeatureTestAttributeHelper.GetDisplayName(group.Key), libraries, features.Values) {
                    Description = this.GetDescription(@group.Key),
                    Scoring = FeatureTestAttributeHelper.GetScoring(@group.Key)
                };

                var resultApplyTasks = new List<Task>();
                foreach (var test in group.OrderBy(FeatureTestAttributeHelper.GetDisplayOrder)) {
                    foreach (var library in libraries) {
                        var cell = table[library, test];
                        var run = testRuns[new { Test = test, LibraryType = library.GetType() }];

                        resultApplyTasks.Add(this.ApplyRunResultToCell(cell, run.Task, featureTestAssembly));
                    }
                }

                Task.WaitAll(resultApplyTasks.ToArray());
                yield return table;
            }
        }

        private async Task ApplyRunResultToCell(FeatureCell cell, Task<FeatureTestResult> resultTask, Assembly testAssembly) {
            var result = await resultTask;
            if (result.Kind == FeatureTestResultKind.Success) {
                cell.DisplayValue = "pass";
                cell.State = FeatureState.Success;
                cell.Details = result.Message;
            }
            else if (result.Kind == FeatureTestResultKind.Failure) {
                var exceptionString = this.exceptionNormalizer.Normalize(result.Exception, testAssembly);

                cell.DisplayValue = "fail";
                cell.State = FeatureState.Failure;
                cell.Details = ConvertExceptionToMarkdown(exceptionString);
                cell.RawError = exceptionString;
            }
            else if (result.Kind == FeatureTestResultKind.SkippedDueToDependency) {
                cell.DisplayValue = "n/a";
                cell.State = FeatureState.Skipped;
                cell.Details = result.Message;
            }
            else {
                cell.DisplayValue = "note";
                cell.State = FeatureState.Concern;
                cell.Details = result.Message;
            }
        }

        private string ConvertExceptionToMarkdown(string exceptionString) {
            return Regex.Replace(exceptionString, "^", "    ", RegexOptions.Multiline);
        }

        private Feature ConvertToFeature(MethodInfo test) {
            return new Feature(test, FeatureTestAttributeHelper.GetDisplayName(test)) { Description = this.GetDescription(test) };
        }

        private string GetDescription(MemberInfo member) {
            var description = member.GetCustomAttributes<DescriptionAttribute>().Select(a => a.Description).SingleOrDefault();
            return this.attributeCleaner.CleanWhitespace(description);
        }
    }
}
