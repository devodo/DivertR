using System;

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

        public Redirect<TTarget> Build(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var redirect = new DelegateCallHandler<TTarget>(callInfo => fastDelegate.Invoke(callInfo.Arguments.InternalArgs));

            return Build(redirect);
        }

        public IVia<TTarget> Redirect(Delegate redirectDelegate)
        {
            var redirect = Build(redirectDelegate);
            
            return InsertRedirect(redirect);
        }
        
        protected Redirect<TTarget> Build(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            _parsedCallExpression.Validate(inputDelegate);
            var redirect = new DelegateCallHandler<TTarget>(mappedRedirect);

            return Build(redirect);
        }

        protected IVia<TTarget> InsertRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return InsertRedirect(redirect);
        }
    }
}
