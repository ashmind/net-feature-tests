using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FeatureTests.Shared.GenericApiSupport {
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

        protected override Expression VisitNew(NewExpression node) {
            var rewrittenConstructor = RewriteMethodIfRequired(node.Constructor);
            if (rewrittenConstructor == node.Constructor)
                return base.VisitNew(node);

            return Expression.New(rewrittenConstructor, node.Arguments);
        }

        private TMethodBase RewriteMethodIfRequired<TMethodBase>(TMethodBase method)
            where TMethodBase : MethodBase
        {
            var rewrittenType = this.rewriteType(method.DeclaringType);
            if (rewrittenType != method.DeclaringType) {
                var newMethod = rewrittenType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                             .OfType<TMethodBase>()
                                             .Single(m => m.MetadataToken == method.MetadataToken);

                if (newMethod.IsGenericMethodDefinition) {
                    var newMethodInfo = (MethodInfo)(object)newMethod;
                    var newGenericArguments = method.GetGenericArguments().Select(this.rewriteType).ToArray();
                    newMethod = (TMethodBase)(object)newMethodInfo.MakeGenericMethod(newGenericArguments);
                }

                return this.RewriteMethodIfRequired(newMethod);
            }

            if (!method.IsGenericMethod)
                return method;

            var arguments = method.GetGenericArguments();
            var rewritten = arguments.Select(this.rewriteType).ToArray();
            if (rewritten.SequenceEqual(arguments))
                return method;

            return (TMethodBase)(object)((MethodInfo)(object)method).GetGenericMethodDefinition().MakeGenericMethod(rewritten);
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

        protected override Expression VisitConstant(ConstantExpression node) {
            var rewritten = this.rewriteType(node.Type);
            if (rewritten == node.Type)
                return base.VisitConstant(node);

            return Expression.Constant(node.Value, rewritten);
        }
    }
}