using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Internal
{
    internal static class CallExpressionParser
    {
        public static ParsedCallExpression FromExpression(Expression expression)
        {
            return expression switch
            {
                MethodCallExpression methodExpression => FromMethodCall(methodExpression),
                MemberExpression propertyExpression => FromPropertyGetter(propertyExpression),
                _ => throw new ArgumentException($"Invalid expression type: {expression.GetType()}", nameof(expression))
            };
        }
        
        public static ParsedCallExpression FromPropertySetter(MemberExpression propertyExpression, Expression valueExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));
            
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member?.GetType()}", nameof(propertyExpression));
            }
            
            var methodInfo = property.GetSetMethod(true);
            var parameterInfos = methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(methodInfo);
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, new[] {valueExpression});

            return new ParsedCallExpression(methodInfo, parameterInfos, methodConstraint, argumentConstraints);
        }
        
        private static ParsedCallExpression FromMethodCall(MethodCallExpression methodExpression)
        {
            var methodInfo = methodExpression?.Method ?? throw new ArgumentNullException(nameof(methodExpression));
            var parameterInfos = methodInfo.GetParameters();
            var argumentConstraints = CreateArgumentConstraints(parameterInfos, methodExpression.Arguments);
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ParsedCallExpression(methodInfo, parameterInfos, methodConstraint, argumentConstraints);
        }

        private static ParsedCallExpression FromPropertyGetter(MemberExpression propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member?.GetType()}", nameof(propertyExpression));
            }
            
            var methodInfo = property.GetGetMethod(true);
            var parameterInfos = methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(methodInfo);
            
            return new ParsedCallExpression(methodInfo, parameterInfos, methodConstraint, Array.Empty<IArgumentConstraint>());
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
            if (argument is ConstantExpression constantExpression)
            {
                return new ConstantArgumentConstraint(constantExpression.Value);
            }

            if (argument is MemberExpression memberExpression)
            {
                if (memberExpression.Member.DeclaringType != null &&
                    memberExpression.Member.DeclaringType.IsGenericType &&
                    memberExpression.Member.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>) &&
                    (memberExpression.Member.Name == nameof(Is<object>.Any) ||
                     memberExpression.Member.Name == nameof(Is<object>.AnyRef) && parameterInfo.ParameterType.IsByRef))
                {
                    return TrueArgumentConstraint.Instance;
                }
            }

            if (argument is MethodCallExpression callExpression &&
                callExpression.Method.DeclaringType != null &&
                callExpression.Method.DeclaringType.IsGenericType &&
                callExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Is<>))
            {
                const BindingFlags activatorFlags = BindingFlags.Public | BindingFlags.Instance;
                var lambdaType = typeof(LambdaArgumentConstraint<>).MakeGenericType(callExpression.Type);
                return (IArgumentConstraint) Activator.CreateInstance(lambdaType, activatorFlags, null, new object[] {callExpression.Arguments[0]}, default);
            }
            
            var value = Expression.Lambda(argument).Compile().DynamicInvoke();
            return new ConstantArgumentConstraint(value);
        }

        private static IMethodConstraint CreateMethodConstraint(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod)
            {
                return new ReferenceMethodConstraint(methodInfo);
            }

            var genericArguments = methodInfo.GetGenericArguments();

            return new GenericMethodConstraint(methodInfo, genericArguments);
        }
    }
}