using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using DependencyInjection.FeatureTables.Generator.Data;
using DependencyInjection.FeatureTables.Generator.Sources.FeatureTestSupport;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Documentation;

namespace DependencyInjection.FeatureTables.Generator.Sources {
    public class FeatureTestTableSource : IFeatureTableSource {
        private readonly FeatureTestRunner runner;

        public FeatureTestTableSource(FeatureTestRunner runner) {
            this.runner = runner;
        }

        public IEnumerable<FeatureTable> GetTables() {
            var testRuns = this.runner.RunAllTests(typeof(BasicTests).Assembly).ToDictionary(r => new { Test = r.Method, r.FrameworkType });
            var testGroups = testRuns.Keys
                                     .Select(k => k.Test)
                                     .Distinct()
                                     .GroupBy(m => m.DeclaringType)
                                     .OrderBy(g => this.GetDisplayOrder(g.Key))
                                     .ToArray();

            foreach (var group in testGroups) {
                var features = group.ToDictionary(m => m, this.ConvertToFeature);
                var table = new FeatureTable(DisplayNameHelper.GetDisplayName(group.Key), Frameworks.List(), features.Values) {
                    Description = this.GetDescription(@group.Key)
                };

                foreach (var test in group.OrderBy(this.GetDisplayOrder)) {
                    foreach (var framework in Frameworks.List()) {
                        var cell = table[framework, test];
                        var run = testRuns[new { Test = test, FrameworkType = framework.GetType() }];

                        ApplyRunResultToCell(cell, run);
                    }
                }

                yield return table;
            }
        }

        private void ApplyRunResultToCell(FeatureCell cell, FeatureTestRun run) {
            if (run.Result == FeatureTestResult.Success) {
                cell.DisplayText = "supported";
                cell.State = FeatureState.Success;
            }
            else if (run.Result == FeatureTestResult.Failure) {
                cell.DisplayText = "failed";
                cell.State = FeatureState.Failure;
                var exceptionString = RemoveLocalPaths(run.Exception.ToString());
                cell.RawError = exceptionString;
                cell.DisplayUri = ConvertToDataUri(exceptionString);
            }
            else if (run.Result == FeatureTestResult.SkippedDueToDependency) {
                cell.DisplayText = "skipped";
                cell.State = FeatureState.Skipped;
            }
            else {
                cell.DisplayText = "see comment";
                cell.State = FeatureState.Concern;
            }
            cell.Comment = run.Message;
        }

        private Feature ConvertToFeature(MethodInfo test) {
            return new Feature(test, DisplayNameHelper.GetDisplayName(test)) { Description = this.GetDescription(test) };
        }

        private string GetDescription(MemberInfo member) {
            var description = member.GetCustomAttributes<DescriptionAttribute>().Select(a => a.Description).SingleOrDefault();
            if (description.IsNullOrEmpty())
                return description;

            // remove all spaces at the start of the line
            return Regex.Replace(description, @"^ +", "", RegexOptions.Multiline);
        }

        private int GetDisplayOrder(MemberInfo member) {
            var displayOrderAttribute = member.GetCustomAttributes<DisplayOrderAttribute>().SingleOrDefault();
            if (displayOrderAttribute == null)
                return int.MaxValue;

            return displayOrderAttribute.Order;
        }
        
        /// <summary>
        /// Removes all paths that are potentially local.
        /// </summary>
        private string RemoveLocalPaths(string exceptionString) {
            return Regex.Replace(exceptionString, @"(?<=\W|^)((?:\w\:|\\\\)[^:\r\n]+)", match => {
                var path = match.Groups[1].Value;
                if (File.Exists(path))
                    return Path.Combine("[removed]", Path.GetFileName(path));

                if (Directory.Exists(path))
                    return "[removed]";

                return match.Value;
            });
        }

        private Uri ConvertToDataUri(string value) {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            return new Uri("data:text/plain;base64," + base64);
        }
    }
}
