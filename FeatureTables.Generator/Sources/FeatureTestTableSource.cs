using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                cell.Text = "supported";
                cell.State = FeatureState.Success;
            }
            else if (run.Result == FeatureTestResult.Failure) {
                cell.Text = "failed";
                cell.State = FeatureState.Failure;
                cell.Uri = ConvertToDataUri(run.Exception);
            }
            else {
                cell.Text = "see comment";
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

            // replace all single new lines with spaces
            description = Regex.Replace(description, @"([^\r\n]|^)(?:\r\n|\r|\n)([^\r\n]|$)", "$1 $2");

            // collapse all spaces
            description = Regex.Replace(description, @" +", @" ");

            // remove all spaces at start/end of the line
            description = Regex.Replace(description, @"^ +| +$", "", RegexOptions.Multiline);
            return description;
        }

        private int GetDisplayOrder(MemberInfo member) {
            var displayOrderAttribute = member.GetCustomAttributes<DisplayOrderAttribute>().SingleOrDefault();
            if (displayOrderAttribute == null)
                return int.MaxValue;

            return displayOrderAttribute.Order;
        }

        private Uri ConvertToDataUri(Exception exception) {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(exception.ToString()));
            return new Uri("data:text/plain;base64," + base64);
        }
    }
}
