using System;
using System.Linq.Expressions;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    internal class ActionRedirectBuilder<T> : RedirectBuilder<T>, IActionRedirectBuilder<T> where T : class
    {
        public ActionRedirectBuilder(IVia<T> via, MemberExpression propertyExpression, Expression valueExpression)
            : base(via, propertyExpression, valueExpression)
        {
        }
        
        public ActionRedirectBuilder(IVia<T> via, MethodCallExpression methodExpression)
            : base(via, methodExpression)
        {
        }
        
        public ActionRedirectBuilder(IVia<T> via, MemberExpression propertyExpression)
            : base(via, propertyExpression)
        {
        }
        
        public IVia<T> To<T1>(Action<T1> redirectDelegate)
        {
            ValidateParameters(redirectDelegate);
            var redirect = new CallRedirect<T>(args =>
            {
                redirectDelegate.Invoke((T1) args[0]);
                return default;
            }, CallConstraint);
            
            return Via.AddRedirect(redirect);
        }
    }
}