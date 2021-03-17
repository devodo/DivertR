using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Core.Internal
{
    internal static class MethodInfoExtensions
    {
        private static readonly ConcurrentDictionary<MethodId, Func<object, object[], object>> DelegateCache =
            new ConcurrentDictionary<MethodId, Func<object, object[], object>>();

        private static readonly ParameterExpression TargetParameterExpression = Expression.Parameter(typeof(object), "target");
        private static readonly ParameterExpression ArgsParameterExpression = Expression.Parameter(typeof(object[]), "args");

        public static Func<object, object[], object> ToDelegate(this MethodInfo methodInfo, Type targetType)
        {
            var methodId = new MethodId(targetType, methodInfo);
            return DelegateCache.GetOrAdd(methodId, mId => mId.MethodInfo.ToDelegateInternal(mId.TargetType));
        }

        static void RefNumber(ref int input)
        {
            input = 2;
        }
        
        private static Func<object, object[], object> ToDelegateInternal2(this MethodInfo methodInfo, Type targetType)
        {
            Func<object, object[], object> f = (instance, args) =>
            {
                int a = (int) args[0];
                RefNumber(ref a);
                args[0] = a;
                return null;
            };

            return f;
        }
        
        private static Func<object, object[], object> ToDelegateInternal(this MethodInfo methodInfo, Type instanceType)
        {
            var argsParameter = Expression.Parameter(typeof(object[]), "args");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");

            var variables = new List<ParameterExpression>();
            var preCallExpressions = new List<Expression>();
            var postCallExpressions = new List<Expression>();
            var parameterInfos = methodInfo.GetParameters();

            var parameterExpressions = new Expression[parameterInfos.Length];
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var indexExpr = Expression.Constant(i, typeof(int));
                
                if (parameterInfos[i].ParameterType.IsByRef)
                {
                    var arrayAccessExpr = Expression.ArrayAccess(
                        argsParameter,
                        indexExpr
                    );
                    
                    var elementType = parameterInfos[i].ParameterType.GetElementType();
                    var variable = Expression.Variable(elementType!);
                    variables.Add(variable);
                    preCallExpressions.Add(Expression.Assign(variable, Expression.Convert(arrayAccessExpr, elementType!)));
                    
                    parameterExpressions[i] = variable;

                    if (parameterInfos[i].IsIn)
                    {
                        continue;
                    }
                    
                    postCallExpressions.Add(Expression.Assign(arrayAccessExpr, Expression.Convert(variable, typeof(object))));
                }
                else
                {
                    var argsIndexed = Expression.ArrayIndex(argsParameter, indexExpr);
                    parameterExpressions[i] = Expression.Convert(argsIndexed, parameterInfos[i].ParameterType);
                }
            }

            var instanceExpression = Expression.Convert(instanceParameter, instanceType);
            //var callExpr = Expression.Call(instanceExpression, methodInfo, parameterExpressions);
            var callExpr = Expression.Invoke(instanceExpression, parameterExpressions);

            Expression resultExpression;
            if (methodInfo.ReturnType != typeof(void))
            {
                resultExpression = Expression.Convert(callExpr, typeof(object));
            }
            else
            {
                var nullObjectExpr = Expression.Constant(null, typeof(object));
                resultExpression = Expression.Block(callExpr, nullObjectExpr);
            }
            
            var resultVariable = Expression.Variable(typeof(object));
            variables.Add(resultVariable);
            var assignExpression = Expression.Assign(resultVariable, resultExpression);
            
            var blockExpression = Expression.Block(variables, preCallExpressions.Append(assignExpression).Concat(postCallExpressions).Append(resultVariable));

            var lambdaExpr = Expression.Lambda(blockExpression, instanceParameter, argsParameter);
            return (Func<object, object[], object>) lambdaExpr.Compile();
        }

        private static Func<object, object[], object> ToDelegateInternalOriginal(this MethodInfo methodInfo, Type targetType)
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
        
        private static MethodInfo MethodInfoFromDelegateType(Type delegateType)
        {
            const string invokeMethod = "Invoke";
            
            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType))
            {
                throw new ArgumentException("Must be a delegate type", nameof(delegateType));
            }

            return delegateType.GetMethod(invokeMethod)!;
        }

        public static T CreateCompatibleDelegate<T>(
            object? firstArgument,
            MethodInfo method)
        {
            MethodInfo delegateInfo = MethodInfoFromDelegateType(typeof(T));

            ParameterInfo[] methodParameters = method.GetParameters();
            ParameterInfo[] delegateParameters = delegateInfo.GetParameters();

            // Convert the arguments from the delegate argument type
            // to the method argument type when necessary.
            ParameterExpression[] arguments =
                (from delegateParameter in delegateParameters
                    select Expression.Parameter(delegateParameter.ParameterType))
                .ToArray();
            Expression[] convertedArguments =
                new Expression[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; ++i)
            {
                Type methodType = methodParameters[i].ParameterType;
                Type delegateType = delegateParameters[i].ParameterType;
                if (methodType != delegateType)
                {
                    convertedArguments[i] =
                        Expression.Convert(arguments[i], methodType);
                }
                else
                {
                    convertedArguments[i] = arguments[i];
                }
            }

            // Create method call.
            var instance = firstArgument == null
                ? null
                : Expression.Constant(firstArgument);
            
            MethodCallExpression methodCall = Expression.Call(
                instance,
                method,
                convertedArguments
            );

            // Convert return type when necessary.
            Expression convertedMethodCall =
                delegateInfo.ReturnType == method.ReturnType
                    ? (Expression) methodCall
                    : Expression.Convert(methodCall, delegateInfo.ReturnType);

            return Expression.Lambda<T>(
                convertedMethodCall,
                arguments
            ).Compile();
        }

        internal readonly struct MethodId : IEquatable<MethodId>
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
                return TargetType == other.TargetType && MethodInfo == other.MethodInfo;
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