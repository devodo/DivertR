using System;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    internal class FuncRedirectBuilder<T, TReturn> : RedirectBuilder<T>, IFuncRedirectBuilder<T, TReturn> where T : class
    {
        public FuncRedirectBuilder(IVia<T> via, MethodCallExpression methodExpression)
            : base(via, methodExpression)
        {
        }
        
        public FuncRedirectBuilder(IVia<T> via, MemberExpression propertyExpression)
            : base(via, propertyExpression)
        {
        }
        
        public IVia<T> To(Func<TReturn> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args => redirectDelegate.Invoke(), CallConstraint);
            
            return Via.AddRedirect(redirect);
        }
        
        
        public IVia<T> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args => redirectDelegate.Invoke((T1) args[0]), CallConstraint);
            
            return Via.AddRedirect(redirect);
        }

        public IVia<T> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args => redirectDelegate.Invoke((T1) args[0], (T2) args[1]), CallConstraint);
            
            return Via.AddRedirect(redirect);
        }
    }
}