using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class RedirectBuilder<T> : IRedirectBuilder<T> where T : class
    {
        protected readonly IVia<T> _via;
        protected readonly ICallCondition? _callCondition;

        public RedirectBuilder(IVia<T> via, LambdaExpression whenExpression)
            : this(via, CreateLambdaCallCondition(whenExpression))
        {
        }

        public RedirectBuilder(IVia<T> via, ICallCondition? callCondition)
        {
            _via = via;
            _callCondition = callCondition;
        }

        public IVia<T> To(T target, object? state = null)
        {
            var redirect = new TargetRedirect<T>(target, state, _callCondition);
            _via.AddRedirect(redirect);
            
            return _via;
        }

        private static ICallCondition CreateLambdaCallCondition(LambdaExpression lambdaExpression)
        {
            return lambdaExpression.Body switch
            {
                MethodCallExpression methodExpression => CreateMethodCallCondition(methodExpression),
                MemberExpression propertyExpression => ParsePropertyCallExpression(propertyExpression),
                //_ => ParseInvocationExpression((InvocationExpression)callExpression.Body),
            };
        }

        private static LambdaCallCondition CreateMethodCallCondition(MethodCallExpression expression)
        {
            var argumentConditions = expression.Arguments.Select(CreateArgumentCondition).ToList();
            
            return new LambdaCallCondition(expression.Method, argumentConditions);
        }
        
        private static LambdaCallCondition ParsePropertyCallExpression(MemberExpression expression)
        {
            if (!(expression.Member is PropertyInfo property))
            {
                throw new ArgumentException("MemberExpression member is not a property", nameof(expression));
            }
            
            return new LambdaCallCondition(property.GetGetMethod(true)!, new List<IArgumentCondition>());
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

    public class RedirectBuilder<T, TResult> : RedirectBuilder<T>, IRedirectBuilder<T, TResult> where T : class
    {
        public RedirectBuilder(IVia<T> via, LambdaExpression whenExpression) : base(via, whenExpression)
        {
        }
        
        public IVia<T> To<T1>(Func<T1, TResult> redirectDelegate)
        {
            var redirect = new CallRedirect<T>(args =>
            {
                var result = redirectDelegate.Invoke((T1) args[0]);
                return result;
            }, _callCondition);
            
            return _via.AddRedirect(redirect);
        }
        
        public IVia<T> To(Func<TResult> redirectDelegate)
        {
            var redirect = new CallRedirect<T>(args =>
            {
                var result = redirectDelegate.Invoke();
                return result;
            }, _callCondition);
            
            return _via.AddRedirect(redirect);
        }
    }
}