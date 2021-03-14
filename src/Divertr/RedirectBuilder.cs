using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    internal class RedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> Via;
        protected readonly ICallCondition CallCondition;
        private readonly MethodInfo? _methodInfo;
        private readonly ParameterInfo[] _parameterInfos = Array.Empty<ParameterInfo>();

        public RedirectBuilder(IVia<T> via, ICallCondition? callCondition = null)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));
            CallCondition = callCondition ?? TrueCallCondition.Instance;
        }
        
        public RedirectBuilder(IVia<T> via, MemberExpression propertyExpression, Expression valueExpression)
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
            var argumentConditions = new[] {CreateArgumentCondition(valueExpression)};
            CallCondition = new LambdaCallCondition(_methodInfo, argumentConditions);
        }

        protected RedirectBuilder(IVia<T> via, MethodCallExpression methodExpression)
        {
            Via = via ?? throw new ArgumentNullException(nameof(via));
            _methodInfo = methodExpression?.Method ?? throw new ArgumentNullException(nameof(methodExpression));
            _parameterInfos = _methodInfo.GetParameters();
            var argumentConditions = methodExpression.Arguments.Select(CreateArgumentCondition).ToArray();
            CallCondition = new LambdaCallCondition(_methodInfo, argumentConditions);
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
            CallCondition = new LambdaCallCondition(_methodInfo, Array.Empty<IArgumentCondition>());
        }
        
        public IVia<T> To(T target, object? state = null)
        {
            var redirect = new TargetRedirect<T>(target, state, CallCondition);
            Via.AddRedirect(redirect);
            
            return Via;
        }

        public IVia<T> To<T1>(Action<T1> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args =>
            {
                redirectDelegate.Invoke((T1) args[0]);
                return default;
            }, CallCondition);
            
            return Via.AddRedirect(redirect);
        }
        
        protected void ValidateParameters(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();
            
            if (!DelegateParametersValid(delegateParameters, _parameterInfos))
            {
                string DelegateToString(ParameterInfo[] parameters)
                {
                    var parameterTypes = parameters.Select(x => x.ParameterType.FullName);
                    return $"{redirectDelegate.Method.ReturnType.FullName} ({string.Join(", ", parameterTypes)})";
                }
                
                throw new DiverterException($"Invalid To delegate '{DelegateToString(delegateParameters)} for redirect expression method '{_methodInfo}'");
            }
        }

        private static bool DelegateParametersValid(ParameterInfo[] delegateParams, ParameterInfo[] callParams)
        {
            if (delegateParams.Length == 0)
            {
                return true;
            }

            if (callParams.Length == 0)
            {
                return false;
            }

            if (delegateParams.Length != callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < delegateParams.Length; i++)
            {
                if (!delegateParams[i].ParameterType.IsAssignableFrom(callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        private static IArgumentCondition CreateArgumentCondition(Expression argument)
        {
            if (argument is ConstantExpression constantExpression)
            {
                return new ConstantArgumentCondition(constantExpression.Value);
            }

            if (argument is MemberExpression memberExpression)
            {
                if (memberExpression.Member.DeclaringType?.GetGenericTypeDefinition() == typeof(Is<>) &&
                    memberExpression.Member.Name == nameof(Is<object>.Any))
                {
                    return TrueArgumentCondition.Instance;
                }
            }

            if (argument is MethodCallExpression callExpression)
            {
                const BindingFlags activatorFlags = BindingFlags.Public | BindingFlags.Instance;
                var lambdaType = typeof(LambdaArgumentCondition<>).MakeGenericType(callExpression.Type);
                return (IArgumentCondition) Activator.CreateInstance(lambdaType, activatorFlags, null, new object[] {callExpression.Arguments[0]}, default);
            }
            
            return FalseArgumentCondition.Instance;
        }
    }
}