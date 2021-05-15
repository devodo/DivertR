using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        protected DelegateRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
            : base(via, parsedCallExpression.ToCallConstraint<TTarget>())
        {
            _parsedCallExpression = parsedCallExpression ?? throw new ArgumentNullException(nameof(parsedCallExpression));
        }

        public IRedirect<TTarget> Build(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var redirect = new DelegateRedirect<TTarget>(callInfo => fastDelegate.Invoke(callInfo.Arguments.InternalArgs), CallConstraint);
            
            return Decorate(redirect);
        }

        public IVia<TTarget> To(Delegate redirectDelegate, int orderWeight = 0)
        {
            var redirect = Build(redirectDelegate);
            
            return InsertRedirect(redirect, orderWeight);
        }
        
        protected IRedirect<TTarget> Build(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            _parsedCallExpression.Validate(inputDelegate);
            var redirect = new DelegateRedirect<TTarget>(mappedRedirect, CallConstraint);

            return Decorate(redirect);
        }

        protected IVia<TTarget> InsertRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect, int orderWeight)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return InsertRedirect(redirect, orderWeight);
        }
    }
}
