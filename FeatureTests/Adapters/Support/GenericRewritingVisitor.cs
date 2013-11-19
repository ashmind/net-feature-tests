using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DependencyInjection.FeatureTests.Adapters.Support {
    public class GenericRewritingVisitor : ExpressionVisitor {
        private readonly Func<Type, Type> rewriteType;

        public GenericRewritingVisitor(Func<Type, Type> rewriteType) {
            this.rewriteType = rewriteType;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {
            var rewrittenMethod = RewriteMethodIfRequired(node.Method);
            if (rewrittenMethod == node.Method)
                return base.VisitMethodCall(node);

            return Expression.Call(Visit(node.Object), rewrittenMethod, Visit(node.Arguments));
        }

        private MethodInfo RewriteMethodIfRequired(MethodInfo method) {
            var rewrittenType = this.rewriteType(method.DeclaringType);
            if (rewrittenType != method.DeclaringType) {
                var newMethod = rewrittenType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                             .Single(m => m.MetadataToken == method.MetadataToken);
                if (newMethod.IsGenericMethodDefinition)
                    newMethod = newMethod.MakeGenericMethod(method.GetGenericArguments().Select(this.rewriteType).ToArray());

                return this.RewriteMethodIfRequired(newMethod);
            }

            var arguments = method.GetGenericArguments();
            var rewritten = arguments.Select(this.rewriteType).ToArray();
            if (rewritten.SequenceEqual(arguments))
                return method;

            return method.GetGenericMethodDefinition().MakeGenericMethod(rewritten);
        }

        protected override Expression VisitUnary(UnaryExpression node) {
            if (node.NodeType != ExpressionType.Convert)
                return base.VisitUnary(node);

            var targetType = node.Type;
            var rewritten = this.rewriteType(targetType);
            if (rewritten == targetType)
                return base.VisitUnary(node);
            
            return Expression.Convert(Visit(node.Operand), rewritten);
        }

        protected override Expression VisitLambda<T>(Expression<T> node) {
            var rewritten = this.rewriteType(typeof(T));
            if (rewritten == typeof(T))
                return base.VisitLambda(node);

            return Expression.Lambda(Visit(node.Body), node.TailCall, VisitAndConvert(node.Parameters, "VisitLambda").ToArray());
        }
    }
}