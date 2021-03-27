using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class FuncRedirectBuilder<T, TReturn> : RedirectBuilder<T>, IFuncRedirectBuilder<T, TReturn> where T : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncRedirectBuilder(IVia<T> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression.ToCallConstraint<T>())
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public override IRedirect<T> BuildRedirect(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);

            return base.BuildRedirect(redirectDelegate);
        }

        public IVia<T> To(TReturn instance)
        {
            return To(() => instance);
        }
        
        public IVia<T> To(Func<TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(args => redirectDelegate.Invoke(), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }

        public IVia<T> To<T1>(Func<T1, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0]), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }

        public IVia<T> To<T1, T2>(Func<T1, T2, TReturn> redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var redirect = new DelegateRedirect<T>(callInfo => redirectDelegate.Invoke((T1) callInfo.Arguments[0], (T2) callInfo.Arguments[1]), BuildCallConstraint());
            
            return Via.AddRedirect(redirect);
        }
    }
}