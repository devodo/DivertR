﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class LambdaExpressionCallInvoker : ICallInvoker
    {
        private readonly ConcurrentDictionary<MethodId, Func<object, object[], object?>> _delegateCache = new();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Invoke<TTarget>(TTarget target, MethodInfo method, CallArguments arguments)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            
            var lambdaDelegate = CreateDelegate(typeof(TTarget), method);

            return lambdaDelegate.Invoke(target, arguments.InternalArgs);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Func<object, object[], object?> CreateDelegate(Type targetType, MethodInfo method)
        {
            var methodId = new MethodId(targetType, method);
            return _delegateCache.GetOrAdd(methodId, id => ToDelegateInternal(id.Method, id.TargetType));
        }

        private static Func<object, object[], object?> ToDelegateInternal(MethodInfo methodInfo, Type targetType, bool isDelegate = false)
        {
            var argsParameter = Expression.Parameter(typeof(object[]), "arguments");
            var targetParameter = Expression.Parameter(typeof(object), "target");

            ByRefState? byRefState = null; // only used if one or more parameters is ByRef
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
                    // It's not possible to cast ref parameters so we need to use a variable
                    var elementType = methodParameters[i].ParameterType.GetElementType()!;
                    var variable = Expression.Variable(elementType);
                    byRefState.Variables.Add(variable);
                    
                    // Assign the ref parameter to the variable (only if it is initialised i.e. not null)
                    var nullCheckExpr = Expression.NotEqual(arrayAccessExpr, Expression.Constant(null, typeof(object)));
                    var assignExpr = Expression.Assign(variable, Expression.Convert(arrayAccessExpr, elementType));
                    var assignConditionExpr = Expression.IfThen(nullCheckExpr, assignExpr);
                    byRefState.PreCall.Add(assignConditionExpr);

                    parameterExpressions[i] = variable;

                    if (methodParameters[i].IsIn)
                    {
                        continue;
                    }

                    // Assign the variable value back to ref and out params after method call
                    byRefState.PostCall.Add(Expression.Assign(arrayAccessExpr, Expression.Convert(variable, typeof(object))));
                }
            }
            
            var targetExpr = Expression.Convert(targetParameter, targetType);

            Expression callExpr = !isDelegate
                ? Expression.Call(targetExpr, methodInfo, parameterExpressions)
                : Expression.Invoke(targetExpr, parameterExpressions);

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
                var tryFinallyExpression = byRefState.PostCall.Count > 0
                    ? (Expression) Expression.TryFinally(assignExpression, Expression.Block(byRefState.PostCall))
                    : assignExpression;
                var blockExpressions = byRefState.PreCall
                    .Append(tryFinallyExpression)
                    .Append(resultVariable);

                lambdaBodyExpr = Expression.Block(byRefState.Variables, blockExpressions);
            }

            var lambdaExpr = Expression.Lambda(lambdaBodyExpr, targetParameter, argsParameter);

            return (Func<object, object[], object?>) lambdaExpr.Compile();
        }

        private class ByRefState
        {
            public List<ParameterExpression> Variables { get; } = new();
            public List<Expression> PreCall { get; } = new();
            public List<Expression> PostCall { get; } = new();
        }
        
        private readonly struct MethodId : IEquatable<MethodId>
        {
            private readonly (Type type, ReferenceEquatable<MethodInfo> method) _id;

            public MethodId(Type targetType, MethodInfo method)
            {
                _id = (targetType, new ReferenceEquatable<MethodInfo>(method));
            }

            public MethodInfo Method => _id.method.Value;
            
            public Type TargetType => _id.type;

            public bool Equals(MethodId other)
            {
                return _id.Equals(other._id);
            }

            public override bool Equals(object? obj)
            {
                return obj is MethodId other && Equals(other);
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }
        }

        private readonly struct ReferenceEquatable<T> : IEquatable<ReferenceEquatable<T>> where T : class
        {
            private readonly T _value;

            public ReferenceEquatable(T value)
            {
                _value = value;
            }

            public T Value => _value;
            
            public bool Equals(ReferenceEquatable<T> other)
            {
                return ReferenceEquals(this._value, other._value);
            }

            public override bool Equals(object? obj)
            {
                return obj is ReferenceEquatable<T> other && Equals(other);
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(_value);
            }
        }
    }
}