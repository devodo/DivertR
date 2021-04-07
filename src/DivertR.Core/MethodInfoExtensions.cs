using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Core
{
    public static class MethodInfoExtensions
    {
        private static readonly ConcurrentDictionary<MethodId, Func<object, object[], object>> DelegateCache =
            new ConcurrentDictionary<MethodId, Func<object, object[], object>>();
        
        public static Func<object, object[], object> ToDelegate<T>(this MethodInfo methodInfo)
        {
            return methodInfo.ToDelegate(typeof(T));
        }

        public static Func<object, object[], object> ToDelegate(this MethodInfo methodInfo, Type targetType)
        {
            var methodId = new MethodId(targetType, methodInfo);
            return DelegateCache.GetOrAdd(methodId, mId => mId.MethodInfo.ToDelegateInternal(mId.TargetType));
        }
        
        public static Func<object[], object> ToDelegate(this Delegate targetDelegate)
        {
            var methodInfo = targetDelegate.Method;
            var targetType = targetDelegate.GetType();
            var methodId = new MethodId(targetType, methodInfo);
            
            var fastDelegate =
                DelegateCache.GetOrAdd(methodId, mId => mId.MethodInfo.ToDelegateInternal(mId.TargetType, isDelegate: true));

            return args => fastDelegate.Invoke(targetDelegate, args);
        }

        private static Func<object, object[], object> ToDelegateInternal(this MethodInfo methodInfo, Type targetType, bool isDelegate = false)
        {
            var argsParameter = Expression.Parameter(typeof(object[]), "arguments");
            var targetParameter = Expression.Parameter(typeof(object), "target");

            ByRefState? byRefState = null;
            var methodParameters = methodInfo.GetParameters();

            var parameterExpressions = new Expression[methodParameters.Length];
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var indexExpr = Expression.Constant(i, typeof(int));
                
                if (!methodParameters[i].ParameterType.IsByRef)
                {
                    var argsIndexExpr = Expression.ArrayIndex(argsParameter, indexExpr);
                    parameterExpressions[i] = Expression.Convert(argsIndexExpr, methodParameters[i].ParameterType);
                }
                else
                {
                    byRefState ??= new ByRefState();

                    var arrayAccessExpr = Expression.ArrayAccess(
                        argsParameter,
                        indexExpr
                    );

                    var elementType = methodParameters[i].ParameterType.GetElementType()!;
                    var variable = Expression.Variable(elementType!);
                    byRefState.Variables.Add(variable);
                    var nullCheckExpr = Expression.NotEqual(arrayAccessExpr, Expression.Constant(null, typeof(object)));
                    var assignExpr = Expression.Assign(variable, Expression.Convert(arrayAccessExpr, elementType));
                    var assignConditionExpr = Expression.IfThen(nullCheckExpr, assignExpr);
                    byRefState.PreCall.Add(assignConditionExpr);

                    parameterExpressions[i] = variable;

                    if (methodParameters[i].IsIn)
                    {
                        continue;
                    }

                    // assign ref and out params after method call
                    byRefState.PostCall.Add(Expression.Assign(arrayAccessExpr, Expression.Convert(variable, typeof(object))));
                }
            }
            
            var targetExpr = Expression.Convert(targetParameter, targetType);

            Expression callExpr = !isDelegate
                ? Expression.Call(targetExpr, methodInfo, parameterExpressions)
                : (Expression) Expression.Invoke(targetExpr, parameterExpressions);

            Expression callCastExpr;
            if (methodInfo.ReturnType != typeof(void))
            {
                callCastExpr = Expression.Convert(callExpr, typeof(object));
            }
            else
            {
                var nullObjectExpr = Expression.Constant(null, typeof(object));
                callCastExpr = Expression.Block(callExpr, nullObjectExpr);
            }

            Expression lambdaBodyExpr;

            if (byRefState == null)
            {
                lambdaBodyExpr = callCastExpr;
            }
            else
            {
                var resultVariable = Expression.Variable(typeof(object));
                byRefState.Variables.Add(resultVariable);
                var assignExpression = Expression.Assign(resultVariable, callCastExpr);
                var blockExpressions = byRefState.PreCall
                    .Append(assignExpression)
                    .Concat(byRefState.PostCall)
                    .Append(resultVariable);

                lambdaBodyExpr = Expression.Block(byRefState.Variables, blockExpressions);
            }

            var lambdaExpr = Expression.Lambda(lambdaBodyExpr, targetParameter, argsParameter);

            return (Func<object, object[], object>) lambdaExpr.Compile();
        }

        private class ByRefState
        {
            public List<ParameterExpression> Variables { get; } = new List<ParameterExpression>();
            public List<Expression> PreCall { get; } = new List<Expression>();
            public List<Expression> PostCall { get; } = new List<Expression>();
        }

        private readonly struct MethodId : IEquatable<MethodId>
        {
            public Type TargetType { get; }
            public MethodInfo MethodInfo { get; }

            public MethodId(Type targetType, MethodInfo methodInfo)
            {
                TargetType = targetType;
                MethodInfo = methodInfo;
            }

            public bool Equals(MethodId other)
            {
                return ReferenceEquals(TargetType, other.TargetType) &&
                       ReferenceEquals(MethodInfo, other.MethodInfo);
            }

            public override bool Equals(object obj)
            {
                return obj is MethodId other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17 * 31 + TargetType.GetHashCode();
                    hash = hash * 31 + MethodInfo.GetHashCode();
                
                    return hash;
                }
            }
        }
    }
}