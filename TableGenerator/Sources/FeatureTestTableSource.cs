using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.Documentation;
using DependencyInjection.FeatureTests.XunitSupport;
using DependencyInjection.TableGenerator.Data;

namespace DependencyInjection.TableGenerator.Sources {
    public class FeatureTestTableSource : IFeatureTableSource {
        public IEnumerable<FeatureTable> GetTables() {
            // potentially I could have used Xunit runners, but they are a bit annoying to get through NuGet
            var testGroups = typeof(BasicTests).Assembly.GetTypes()
                                                        .SelectMany(t => t.GetMethods())
                                                        .Where(m => m.IsDefined<ForEachFrameworkAttribute>(false))
                                                        .GroupBy(m => m.DeclaringType)
                                                        .OrderBy(g => GetDisplayOrder(g.Key))
                                                        .ToArray();

            foreach (var group in testGroups) {
                var groupSpecialCases = GetSpecialCases(group.Key);
                var features = group.ToDictionary(m => m, this.ConvertToFeature);
                var table = new FeatureTable(GetDisplayName(group.Key), Frameworks.List(), features.Values) {
                    Description = this.GetDescription(@group.Key)
                };

                foreach (var test in group.OrderBy(GetDisplayOrder)) {
                    var specialCases = GetSpecialCases(test);
                    specialCases = specialCases.Concat(groupSpecialCases.Where(p => !specialCases.ContainsKey(p.Key)))
                                               .ToDictionary(p => p.Key, p => p.Value);
                    
                    foreach (var framework in Frameworks.List()) {
                        var cell = table[framework, features[test]];
                        var specialCaseText = specialCases.GetValueOrDefault(framework.GetType());
                        if (specialCaseText != null) {
                            cell.Text = "see comment";
                            cell.Comment = specialCaseText;
                            cell.State = FeatureState.Neutral;
                            continue;
                        }

                        RunTestAndCollectResult(test, framework, cell);
                    }
                }

                yield return table;
            }
        }

        private Feature ConvertToFeature(MethodInfo test) {
            return new Feature(test, this.GetDisplayName(test)) { Description = this.GetDescription(test) };
        }

        private void RunTestAndCollectResult(MethodInfo test, IFrameworkAdapter framework, FeatureCell cell) {
            var instance = Activator.CreateInstance(test.DeclaringType);
            try {
                test.Invoke(instance, new object[] {framework});
            }
            catch (Exception ex) {
                CollectFailure(cell, ex);
                return;
            }

            cell.Text = "supported";
            cell.State = FeatureState.Success;
        }

        private static void CollectFailure(FeatureCell cell, Exception exception) {
            cell.Text = "failed";
            cell.State = FeatureState.Failure;
            cell.Comment = GetUsefulMessage(exception);
        }

        private static string GetUsefulMessage(Exception exception) {
            var invocationException = exception as TargetInvocationException;
            if (invocationException != null)
                return GetUsefulMessage(invocationException.InnerException);
            
            return exception.Message;
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

        private string GetDisplayName(MemberInfo member) {
            var displayNameAttribute = member.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault();
            if (displayNameAttribute == null)
                return member.Name;

            return displayNameAttribute.DisplayName;
        }

        private int GetDisplayOrder(MemberInfo member) {
            var displayOrderAttribute = member.GetCustomAttributes<DisplayOrderAttribute>().SingleOrDefault();
            if (displayOrderAttribute == null)
                return int.MaxValue;

            return displayOrderAttribute.Order;
        }

        private IDictionary<Type, string> GetSpecialCases(MemberInfo member) {
            return member.GetCustomAttributes<SpecialCaseAttribute>().ToDictionary(a => a.FrameworkType, a => a.Comment);
        }
    }
}
