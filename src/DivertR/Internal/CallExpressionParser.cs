using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Internal
{
    internal static class CallExpressionParser
    {
        public static ICallValidator FromExpression(Expression expression)
        {
            return expression switch
            {
                MethodCallExpression methodExpression => FromMethodCall(methodExpression),
                MemberExpression propertyExpression => FromProperty(propertyExpression),
                _ => throw new ArgumentException($"Invalid expression type: {expression.GetType()}", nameof(expression))
            };
        }
        
        public static ICallValidator FromPropertySetter(MemberExpression propertyExpression, Expression valueExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));
            
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member.GetType()}", nameof(propertyExpression));
            }
            
            var methodInfo = property.GetSetMethod(true);
            var parameterInfos = methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(methodInfo);
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, new[] { valueExpression });

            return new ExpressionCallValidator(methodInfo, parameterInfos, methodConstraint, argumentConstraints);
        }
        
        private static ExpressionCallValidator FromMethodCall(MethodCallExpression methodExpression)
        {
            var methodInfo = methodExpression.Method ?? throw new ArgumentNullException(nameof(methodExpression));
            var parameterInfos = methodInfo.GetParameters();
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, methodExpression.Arguments);
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ExpressionCallValidator(methodInfo, parameterInfos, methodConstraint, argumentConstraints);
        }

        private static ICallValidator FromProperty(MemberExpression propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member.GetType()}", nameof(propertyExpression));
            }

            if (property.DeclaringType?.IsGenericType == true &&
                property.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>))
            {
                if (property.Name != nameof(Is<object>.Return))
                {
                    throw new ArgumentException($"Only the property Is<T>.{nameof(Is<object>.Return)} may be used as expression return value");
                }
                
                return new ReturnCallValidator(property.PropertyType);
            }
            
            var methodInfo = property.GetGetMethod(true);
            var parameterInfos = methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ExpressionCallValidator(methodInfo, parameterInfos, methodConstraint, Array.Empty<IArgumentConstraint>());
        }
        
        private static IArgumentConstraint[] CreateArgumentConstraints(ParameterInfo[] parameterInfos, IReadOnlyCollection<Expression> arguments)
        {
            if (parameterInfos.Length != arguments.Count)
            {
                throw new ArgumentException("Number of parameters does not match number of arguments", nameof(parameterInfos));
            }

            return arguments
                .Select((arg, i) => CreateArgumentConstraint(parameterInfos[i], arg))
                .ToArray();
        }

        private static IArgumentConstraint CreateArgumentConstraint(ParameterInfo parameterInfo, Expression argument)
        {
            switch (argument)
            {
                case ConstantExpression constantExpression:
                    return new ConstantArgumentConstraint(constantExpression.Value);

                case MemberExpression memberExpression
                    when memberExpression.Member.DeclaringType != null &&
                         memberExpression.Member.DeclaringType.IsGenericType &&
                         (memberExpression.Member.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>) ||
                          memberExpression.Member.DeclaringType.GetGenericTypeDefinition() == typeof(IsRef<>) &&
                          parameterInfo.ParameterType.IsByRef) &&
                         memberExpression.Member.Name == nameof(Is<object>.Any):
                    {
                        return BuildTypeMatchConstraint(memberExpression.Member.DeclaringType.GenericTypeArguments[0]);
                    }

                case MethodCallExpression callExpression
                    when callExpression.Method.DeclaringType != null &&
                         callExpression.Method.DeclaringType.IsGenericType &&
                         callExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>) &&
                         callExpression.Arguments.Any() &&
                         callExpression.Arguments[0] is LambdaExpression lambdaExpression &&
                         lambdaExpression.ReturnType == typeof(bool):
                    {
                        return BuildLambdaMatchConstraint(callExpression.Type, lambdaExpression);
                    }
                
                case MemberExpression { Expression: MethodCallExpression callExpression }
                    when callExpression.Method.DeclaringType != null &&
                         callExpression.Method.DeclaringType.IsGenericType &&
                         callExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(IsRef<>) &&
                         parameterInfo.ParameterType.IsByRef &&
                         callExpression.Type.IsGenericType && 
                         callExpression.Type.GetGenericTypeDefinition() == typeof(RefValue<>) &&
                         callExpression.Arguments.Any() &&
                         callExpression.Arguments[0] is LambdaExpression lambdaExpression &&
                         lambdaExpression.ReturnType == typeof(bool):
                    {
                        return BuildLambdaMatchConstraint(callExpression.Type.GenericTypeArguments[0], lambdaExpression);
                    }

                case MethodCallExpression callExpression
                    when callExpression.Method.DeclaringType?.FullName == "Moq.It":
                    {
                        throw new ArgumentException("Moq.It argument syntax is not supported");
                    }
                
                default:
                    {
                        var value = Expression.Lambda(argument).Compile().DynamicInvoke();
                        
                        return new ConstantArgumentConstraint(value);
                    }
            }
        }

        private static IArgumentConstraint BuildLambdaMatchConstraint(Type argumentType, LambdaExpression matchExpression)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
            var lambdaType = typeof(LambdaArgumentConstraint<>).MakeGenericType(argumentType);
            
            return (IArgumentConstraint) Activator.CreateInstance(lambdaType, ActivatorFlags, null, new object[] { matchExpression }, default);
        }
        
        private static readonly ConcurrentDictionary<Type, IArgumentConstraint> TypeMatchConstraintCache = new ConcurrentDictionary<Type, IArgumentConstraint>();
        
        private static IArgumentConstraint BuildTypeMatchConstraint(Type argumentType)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;

            return TypeMatchConstraintCache.GetOrAdd(argumentType, argType =>
            {
                var lambdaType = typeof(TypeArgumentConstraint<>).MakeGenericType(argumentType);

                return (IArgumentConstraint) Activator.CreateInstance(lambdaType, ActivatorFlags, null, null, default);
            });
        }

        private static IMethodConstraint CreateMethodConstraint(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod)
            {
                return new EqualsMethodConstraint(methodInfo);
            }

            return new GenericMethodConstraint(methodInfo);
        }
    }
}