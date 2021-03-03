using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Internal
{
    internal static class MethodInfoExtensions
    {
        private static readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> DelegateCache =
            new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();
        private static readonly ParameterExpression TargetParameterExpression = Expression.Parameter(typeof(object), "target");
        private static readonly ParameterExpression ArgsParameterExpression = Expression.Parameter(typeof(object[]), "args");

        public static Func<object, object[], object> ToDelegate(this MethodInfo methodInfo, Type targetType)
        {
            return DelegateCache.GetOrAdd(methodInfo, m => m.ToDelegateInternal(targetType));
        }
        
        private static Func<object, object[], object> ToDelegateInternal(this MethodInfo methodInfo, Type targetType)
        {
            var parameters = methodInfo.GetParameters();

            var parameterExpressions = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var indexExpr = Expression.Constant(i, typeof(int));
                var argExpr = Expression.ArrayIndex(ArgsParameterExpression, indexExpr);
                parameterExpressions[i] = Expression.Convert(argExpr, parameters[i].ParameterType);
            }
            
            var targetCastExpr = Expression.Convert(TargetParameterExpression, targetType);
            var callExpr = Expression.Call(targetCastExpr, methodInfo, parameterExpressions);
            
            Expression resultExpr;
            if (methodInfo.ReturnType != typeof(void))
            {
                resultExpr = Expression.Convert(callExpr, typeof(object));
            }
            else
            {
                var nullObjectExpr = Expression.Constant(null, typeof(object));
                resultExpr = Expression.Block(callExpr, nullObjectExpr);
            }

            var lambdaExpr = Expression.Lambda(resultExpr, TargetParameterExpression, ArgsParameterExpression);
            return (Func<object, object[], object>) lambdaExpr.Compile();
        }
    }
}