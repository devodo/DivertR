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
        public static ICallValidator FromExpression<TTarget>(Expression expression)
        {
            return expression switch
            {
                MethodCallExpression methodExpression => FromMethodCall(methodExpression, typeof(TTarget)),
                MemberExpression propertyExpression => FromProperty(propertyExpression, typeof(TTarget)),
                _ => throw new ArgumentException($"Invalid expression type: {expression.GetType()}", nameof(expression))
            };
        }
        
        public static ICallValidator FromProperty(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return FromProperty(memberExpression, null);
            }

            throw new ArgumentException($"Must be a property MemberExpression but got: {expression.GetType()}", nameof(expression));
        }
        
        public static ICallValidator FromPropertySetter<TTarget>(MemberExpression propertyExpression, Expression valueExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Member is not PropertyInfo property)
            {
                throw new ArgumentException($"The expression Member property must be of type PropertyInfo but got: {propertyExpression.Member.GetType()}", nameof(propertyExpression));
            }
            
            var methodInfo = property.GetSetMethod(true);
            ValidateCanRedirect(methodInfo, typeof(TTarget));
            var parameterInfos = methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(methodInfo);
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, new[] { valueExpression });

            return new ExpressionCallValidator(methodInfo, methodConstraint, argumentConstraints);
        }
        
        private static ExpressionCallValidator FromMethodCall(MethodCallExpression expression, Type targetType)
        {
            var methodInfo = expression.Method ?? throw new ArgumentNullException(nameof(expression));

            if (methodInfo.DeclaringType == null || !methodInfo.DeclaringType.IsAssignableFrom(targetType))
            {
                throw new ArgumentException($"The method declaring type {methodInfo.DeclaringType} is not assignable from the Via target type: {targetType}", nameof(expression));
            }

            ValidateCanRedirect(methodInfo, targetType);
            
            var parameterInfos = methodInfo.GetParameters();
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, expression.Arguments);
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ExpressionCallValidator(methodInfo, methodConstraint, argumentConstraints);
        }

        private static ICallValidator FromProperty(MemberExpression expression, Type? targetType)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            
            if (expression.Member is not PropertyInfo property)
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {expression.Member.GetType()}", nameof(expression));
            }

            if (property.DeclaringType?.IsGenericType == true &&
                property.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>))
            {
                if (property.Name != nameof(Is<object>.Return))
                {
                    throw new ArgumentException($"Only Is<T>.{nameof(Is<object>.Return)} may be used as a return constraint property", nameof(expression));
                }
                
                return new ReturnCallValidator(property.PropertyType);
            }

            if (targetType == null)
            {
                throw new ArgumentException($"Only the property Is<T>.{nameof(Is<object>.Return)} may be used as a return constraint value in this context", nameof(expression));
            }
            
            if (property.DeclaringType == null || !property.DeclaringType.IsAssignableFrom(targetType))
            {
                throw new ArgumentException($"The property declaring type {property.DeclaringType} is not assignable from the Via target type: {targetType}", nameof(expression));
            }

            var methodInfo = property.GetGetMethod(true);
            ValidateCanRedirect(methodInfo, targetType);
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ExpressionCallValidator(methodInfo, methodConstraint, Array.Empty<IArgumentConstraint>());
        }

        private static void ValidateCanRedirect(MethodInfo methodInfo, Type targetType)
        {
            if (targetType.IsInterface)
            {
                return;
            }

            if ((methodInfo.IsVirtual && !methodInfo.IsFinal) || methodInfo.IsAbstract)
            {
                return;
            }
            
            throw new DiverterValidationException($"Member '{methodInfo.Name}' cannot be redirected. Only interface members or virtual or abstract class members are valid");
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

        private static bool IsDiverterArgument(MemberInfo member, ParameterInfo parameterInfo)
        {
            if (member.DeclaringType is not { IsGenericType: true })
            {
                return false;
            }

            var typeDefinition = member.DeclaringType.GetGenericTypeDefinition();

            if (typeDefinition == typeof(Is<>))
            {
                return true;
            }

            if (typeDefinition == typeof(IsRef<>))
            {
                if (!parameterInfo.ParameterType.IsByRef)
                {
                    throw new DiverterValidationException($"IsRef<T> argument used with parameter '{parameterInfo}' that is not ByRef");
                }

                return true;
            }

            return false;
        }

        private static IArgumentConstraint CreateArgumentConstraint(ParameterInfo parameterInfo, Expression argument)
        {
            switch (argument)
            {
                case ConstantExpression constantExpression:
                    return new ConstantArgumentConstraint(parameterInfo, constantExpression.Value);
                
                case MemberExpression memberExpression
                    when IsDiverterArgument(memberExpression.Member, parameterInfo):
                    {
                        return BuildTypeMatchConstraint(parameterInfo, memberExpression.Member);
                    }
                
                case UnaryExpression { Operand: MemberExpression memberExpression }
                    when IsDiverterArgument(memberExpression.Member, parameterInfo):
                    {
                        return BuildTypeMatchConstraint(parameterInfo, memberExpression.Member);
                    }

                case MethodCallExpression callExpression
                    when IsDiverterArgument(callExpression.Method, parameterInfo) &&
                         callExpression.Arguments.Any() &&
                         callExpression.Arguments[0] is LambdaExpression lambdaExpression &&
                         lambdaExpression.ReturnType == typeof(bool):
                    {
                        return BuildLambdaMatchConstraint(parameterInfo, callExpression.Type, lambdaExpression);
                    }
                
                case UnaryExpression { Operand: MethodCallExpression callExpression }
                    when IsDiverterArgument(callExpression.Method, parameterInfo) &&
                         callExpression.Arguments.Any() &&
                         callExpression.Arguments[0] is LambdaExpression lambdaExpression &&
                         lambdaExpression.ReturnType == typeof(bool):
                    {
                        return BuildLambdaMatchConstraint(parameterInfo, callExpression.Type, lambdaExpression);
                    }
                
                case MemberExpression { Expression: MethodCallExpression callExpression }
                    when IsDiverterArgument(callExpression.Method, parameterInfo) &&
                         callExpression.Type.IsGenericType && 
                         callExpression.Type.GetGenericTypeDefinition() == typeof(RefValue<>) &&
                         callExpression.Arguments.Any() &&
                         callExpression.Arguments[0] is LambdaExpression lambdaExpression &&
                         lambdaExpression.ReturnType == typeof(bool):
                    {
                        return BuildLambdaMatchConstraint(parameterInfo, callExpression.Type.GenericTypeArguments[0], lambdaExpression);
                    }

                case MethodCallExpression callExpression
                    when callExpression.Method.DeclaringType?.FullName == "Moq.It":
                    {
                        throw new DiverterValidationException("Moq.It argument match syntax is not supported by DivertR");
                    }
                
                default:
                    {
                        var value = Expression.Lambda(argument).Compile().DynamicInvoke();
                        
                        return new ConstantArgumentConstraint(parameterInfo, value);
                    }
            }
        }
        
        private static IArgumentConstraint BuildLambdaMatchConstraint(ParameterInfo parameterInfo, Type argumentType, LambdaExpression matchExpression)
        {
            var constraintFactory = GetArgumentConstraintFactory(argumentType);

            return constraintFactory.CreateLambdaArgumentConstraint(parameterInfo, matchExpression);
        }

        private static IArgumentConstraint BuildTypeMatchConstraint(ParameterInfo parameterInfo, MemberInfo member)
        {
            if (member.Name != nameof(Is<object>.Any))
            {
                throw new DiverterValidationException($"Only the property Is<T>.{nameof(Is<object>.Any)} may be used in this context as a method argument value");
            }
                        
            var argumentType = member.DeclaringType!.GenericTypeArguments[0];
            var constraintFactory = GetArgumentConstraintFactory(argumentType);

            return constraintFactory.CreateTypeArgumentConstraint(parameterInfo);
        }
        
        private static readonly ConcurrentDictionary<Type, IArgumentConstraintFactory> ArgumentConstraintFactoryCache = new();

        private static IArgumentConstraintFactory GetArgumentConstraintFactory(Type argumentType)
        {
            const BindingFlags ActivatorFlags = BindingFlags.Public | BindingFlags.Instance;
            
            return ArgumentConstraintFactoryCache.GetOrAdd(argumentType, argType =>
            {
                var lambdaType = typeof(ArgumentConstraintFactory<>).MakeGenericType(argType);

                return (IArgumentConstraintFactory) Activator.CreateInstance(lambdaType, ActivatorFlags, null, null, default);
            });
        }

        private static IMethodConstraint CreateMethodConstraint(MethodInfo methodInfo)
        {
            return new SameMethodConstraint(methodInfo);
        }
    }
}