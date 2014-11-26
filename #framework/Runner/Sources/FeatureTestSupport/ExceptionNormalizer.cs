using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using FeatureTests.Shared;

namespace FeatureTests.Runner.Sources.FeatureTestSupport {
    public class ExceptionNormalizer {
        public string Normalize(Exception exception, Assembly testAssembly) {
            var assembliesToStop = new HashSet<Assembly> {testAssembly, typeof (ILibrary).Assembly};
            var builder = new StringBuilder();
            var exceptions = new Stack<Exception>();

            var inner = exception;
            while (inner != null) {
                exceptions.Push(inner);
                if (inner != exception)
                    builder.Append(" ---> ");

                builder.Append(inner.GetType().FullName).Append(": ").Append(inner.Message);
                inner = inner.InnerException;
            }

            foreach (var outer in exceptions) {
                var frames = new StackTrace(outer).GetFrames();
                if (frames == null)
                    continue;

                builder.AppendLine();
                foreach (var frame in frames) {
                    var method = frame.GetMethod();
                    if (assembliesToStop.Contains(method.Module.Assembly))
                        break;

                    builder.Append("   at ");
                    AppendMethod(builder, method);
                    builder.AppendLine();
                }
                if (outer != exception)
                    builder.Append("   --- End of inner exception stack trace ---");
            }

            return builder.ToString();
        }

        private void AppendMethod(StringBuilder builder, MethodBase method) {
            if (method.DeclaringType != null) {
                AppendType(builder, method.DeclaringType);
                builder.Append(".");
            }
            AppendName(builder, method.Name);
            if (method.IsGenericMethod) {
                builder.Append("[");
                method.GetGenericArguments().ForEach((type, index) => {
                    if (index > 0)
                        builder.Append(",");
                    AppendType(builder, type);
                });
                builder.Append("]");
            }
            builder.Append("(");
            method.GetParameters().ForEach((parameter, index) => {
                if (index > 0)
                    builder.Append(", ");

                builder.Append(parameter.ParameterType.Name)
                       .Append(" ")
                       .Append(parameter.Name);
            });
            builder.Append(")");
        }

        private void AppendType(StringBuilder builder, Type type) {
            if (type.IsGenericParameter) {
                AppendName(builder, type.Name);
                return;
            }
            
            var types = new Stack<Type>();
            var current = type;
            while (current != null) {
                types.Push(current);
                current = current.DeclaringType;
            }

            AppendName(builder, types.Pop().FullName);
            foreach (var nested in types) {
                builder.Append(".");
                AppendName(builder, nested.Name);
            }
        }

        private void AppendName(StringBuilder builder, string name) {
            builder.Append(CleanupIfRequired(name));
        }

        private string CleanupIfRequired(string name) {
            // guid-like
            return Regex.Replace(name, @"(?:[a-f\d]|[A-F\d]){32,}", "[..]");
        }
    }
}
