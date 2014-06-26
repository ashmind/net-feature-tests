using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using FeatureTests.Shared.GenericApiSupport.GenericPlaceholders;

namespace FeatureTests.Shared.GenericApiSupport {
    public static class GenericHelper {
        public static void RewriteAndInvoke(Expression<Action> call, params Type[] genericArguments) {
            var rewritten = Rewrite(call, genericArguments);
            ((Action)rewritten.Compile())();
        }

        public static object RewriteAndInvoke<TResult>(Expression<Func<TResult>> call, params Type[] genericArguments) {
            var rewritten = Rewrite(call, genericArguments);
            return ((Func<object>)rewritten.Compile())();
        }

        private static LambdaExpression Rewrite<TDelegate>(Expression<TDelegate> call, Type[] genericArguments) {
            var visitor = new GenericRewritingVisitor(a => RewriteTypeIfPossible(a, genericArguments));
            return (LambdaExpression)visitor.Visit(call);
        }

        private static Type RewriteTypeIfPossible(Type type, Type[] rewriteTo) {
            if (type.IsDefined<GenericPlaceholderAttribute>())
                return RewritePlaceholderType(type, rewriteTo);

            if (!type.IsGenericType)
                return type;

            var arguments = type.GetGenericArguments();
            var rewritten = arguments.Select(a => RewriteTypeIfPossible(a, rewriteTo)).ToArray();
            if (!rewritten.SequenceEqual(arguments))
                return type.GetGenericTypeDefinition().MakeGenericType(rewritten);

            return type;
        }

        private static Type RewritePlaceholderType(Type type, Type[] rewriteTo) {
            if (type.IsGenericType)
                type = type.GetGenericArguments()[0];

            var index = int.Parse(Regex.Match(type.Name, @"\d+").Value) - 1;
            return rewriteTo[index];
        }
    }
}
