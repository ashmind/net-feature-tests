using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using DependencyInjection.FeatureTests;
using DependencyInjection.FeatureTests.Adapters;
using DependencyInjection.FeatureTests.XunitSupport;
using DependencyInjection.TableGenerator.Data;
using Microsoft.Practices.ServiceLocation;

namespace DependencyInjection.TableGenerator.Sources {
    public class FeatureTestTableSource : IFeatureTableSource {
        public IEnumerable<FeatureTable> GetTables() {
            // potentially I could have used Xunit runners, but they are a bit annoying to use through Nuget
            var testGroups = typeof(BasicTests).Assembly.GetTypes()
                                                        .SelectMany(t => t.GetMethods())
                                                        .Where(m => m.IsDefined<ForEachFrameworkAttribute>(false))
                                                        .GroupBy(m => m.DeclaringType)
                                                        .OrderBy(g => g.Key.Name)
                                                        .ToArray();

            foreach (var group in testGroups) {
                var table = new FeatureTable(group.Key.Name, Frameworks.List(), group.Select(m => m.Name));
                foreach (var test in group) {
                    foreach (var framework in Frameworks.List()) {
                        RunTestAndCollectResult(test, framework, table[framework, test.Name]);
                    }
                }

                yield return table;
            }
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

            var activationException = exception as ActivationException;
            if (activationException != null)
                return GetUsefulMessage(activationException.InnerException);

            return exception.Message;
        }
    }
}
