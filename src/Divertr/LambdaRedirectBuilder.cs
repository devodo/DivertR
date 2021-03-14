using System;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    internal class LambdaRedirectBuilder<T, TReturn> : RedirectBuilder<T>, IRedirectBuilder<T, TReturn> where T : class
    {
        public LambdaRedirectBuilder(IVia<T> via, MethodCallExpression methodExpression)
            : base(via, methodExpression)
        {
        }
        
        public LambdaRedirectBuilder(IVia<T> via, MemberExpression propertyExpression)
            : base(via, propertyExpression)
        {
        }
        
        public IVia<T> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args => redirectDelegate.Invoke((T1) args[0]), CallCondition);
            
            return Via.AddRedirect(redirect);
        }
        
        public IVia<T> To(Func<TReturn> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args => redirectDelegate.Invoke(), CallCondition);
            
            return Via.AddRedirect(redirect);
        }
    }
}