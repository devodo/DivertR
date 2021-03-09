using System;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class WhenBuilder<T, TResult> where T : class
    {
        private readonly IVia<T> _via;
        private ICallCondition _callCondition;
        private MethodInfo _methodInfo;
        
        public WhenBuilder(IVia<T> via, LambdaExpression whenExpression)
        {
            _via = via;
            _callCondition = whenExpression.Body switch
            {
                MethodCallExpression methodExpression => CreateMethodCallCondition(methodExpression),
                //MemberExpression propertyExpression => ParsePropertyCallExpression(propertyExpression),
                //_ => ParseInvocationExpression((InvocationExpression)callExpression.Body),
            };
        }

        public IVia<T> Redirect<T1>(Func<T1, TResult> redirectDelegate)
        {
            var redirect = new CallRedirect<T>(args =>
            {
                var result = redirectDelegate.Invoke((T1) args[0]);
                return result;
            }, _callCondition);
            return _via.Redirect(redirect);
        }

        private LambdaCallCondition CreateMethodCallCondition(MethodCallExpression expression)
        {
            _methodInfo = expression.Method;
            return new LambdaCallCondition(expression.Method, expression.Arguments, expression.Method.GetParameters());
        }
    }
}