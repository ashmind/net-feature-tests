using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DependencyInjection.FeatureTests.Adapters.Support.GenericPlaceholders;

namespace DependencyInjection.FeatureTests.Adapters.Support {
    public static class GenericHelper {
        public static void RewriteAndInvoke(Expression<Action> call, params Type[] genericArguments) {
            var rewritten = new GenericRewritingVisitor(a => RewriteTypeIfPossible(a, genericArguments))
                                    .VisitAndConvert(call, "RewriteAndInvoke");
            rewritten.Compile()();
        }

        private static Type RewriteTypeIfPossible(Type type, Type[] rewriteTo) {
            if (type.Namespace == typeof(X1).Namespace)
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

            var index = int.Parse(type.Name.Substring(1)) - 1;
            return rewriteTo[index];
        }
    }
}
