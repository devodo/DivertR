using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;
using DivertR.Internal;

namespace DivertR
{
    internal class RedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> Via;
        protected readonly List<ICallConstraint> CallConstraints = new List<ICallConstraint>();
        private readonly MethodInfo? _methodInfo;
        private readonly ParameterInfo[] _parameterInfos = Array.Empty<ParameterInfo>();

        public RedirectBuilder(IVia<T> via, ICallConstraint? callConstraint = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                CallConstraints.Add(callConstraint);
            }
        }

        protected RedirectBuilder(IVia<T> via, MemberExpression propertyExpression, Expression valueExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));
            
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member?.GetType()}", nameof(propertyExpression));
            }
            
            Via = via ?? throw new ArgumentNullException(nameof(via));
            _methodInfo = property.GetSetMethod(true);
            _parameterInfos = _methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(_methodInfo);
            var argumentConstraints = CreateArgumentConstraints(_parameterInfos, new[] {valueExpression});
            CallConstraints.Add(new MethodCallConstraint(_methodInfo, methodConstraint, argumentConstraints));
        }

        protected RedirectBuilder(IVia<T> via, MethodCallExpression methodExpression)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));
            _methodInfo = methodExpression.Method ?? throw new ArgumentNullException(nameof(methodExpression));
            _parameterInfos = _methodInfo.GetParameters();
            var argumentConstraints = CreateArgumentConstraints(_parameterInfos, methodExpression.Arguments);
            var methodConstraint = CreateMethodConstraint(_methodInfo);
            CallConstraints.Add(new MethodCallConstraint(_methodInfo, methodConstraint, argumentConstraints));
        }

        protected RedirectBuilder(IVia<T> via, MemberExpression propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            if (!(propertyExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"Member expression must be of type PropertyInfo but got: {propertyExpression.Member?.GetType()}", nameof(propertyExpression));
            }
            
            Via = via ?? throw new ArgumentNullException(nameof(via));
            _methodInfo = property.GetGetMethod(true);
            _parameterInfos = _methodInfo.GetParameters();
            var methodConstraint = CreateMethodConstraint(_methodInfo);
            CallConstraints.Add(new MethodCallConstraint(_methodInfo, methodConstraint, Array.Empty<IArgumentConstraint>()));
        }
        
        public IVia<T> To(T target, object? state = null)
        {
            var redirect = new TargetRedirect<T>(target, state, CallConstraints);
            Via.AddRedirect(redirect);
            
            return Via;
        }
        
        public IVia<T> To(Delegate redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            ValidateReturnType(redirectDelegate);
            
            var redirect = new CallRedirect<T>(args => 
                redirectDelegate.GetMethodInfo().ToDelegate(redirectDelegate.GetType()).Invoke(redirectDelegate, args), CallConstraints);
            
            return Via.AddRedirect(redirect);
        }

        protected void ValidateParameters(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();
            
            if (!DelegateParametersValid(delegateParameters, _parameterInfos))
            {
                throw new DiverterException(CreateIncompatibleMessage(redirectDelegate, _methodInfo!));
            }
        }

        private void ValidateReturnType(Delegate redirectDelegate)
        {
            var returnType = redirectDelegate.Method.ReturnType;

            if (returnType != _methodInfo!.ReturnType && !_methodInfo.ReturnType.IsAssignableFrom(returnType))
            {
                throw new DiverterException(CreateIncompatibleMessage(redirectDelegate, _methodInfo!));
            }
        }
        
        private static string CreateDelegateSignature(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();
            var parameterTypes = delegateParameters.Select(x => x.ParameterType.FullName);
            return $"{redirectDelegate.Method.ReturnType.FullName} Invoke({string.Join(", ", parameterTypes)})";
        }

        private static string CreateIncompatibleMessage(Delegate redirectDelegate, MethodInfo methodInfo)
        {
            return $"To() delegate '{CreateDelegateSignature(redirectDelegate)}' invalid for redirect method '{methodInfo}'";
        }

        private static bool DelegateParametersValid(ParameterInfo[] delegateParams, ParameterInfo[] callParams)
        {
            if (delegateParams.Length == 0)
            {
                return true;
            }
            
            if (delegateParams.Length != callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < delegateParams.Length; i++)
            {
                if (ReferenceEquals(delegateParams[i].ParameterType, callParams[i].ParameterType))
                {
                    continue;
                }
                
                if (!delegateParams[i].ParameterType.IsAssignableFrom(callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
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