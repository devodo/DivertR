using System;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ParsedCallExpression ParsedCallExpression;

        protected DelegateRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint<TTarget> callConstraint)
            : base(via, callConstraint)
        {
            ParsedCallExpression = parsedCallExpression ?? throw new ArgumentNullException(nameof(parsedCallExpression));
        }

        public Redirect<TTarget> Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            ParsedCallExpression.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var redirect = new DelegateCallHandler<TTarget>(callInfo => fastDelegate.Invoke(callInfo.Arguments.InternalArgs));

            return Build(redirect, optionsAction);
        }

        public IDelegateRedirectBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        protected IVia<TTarget> InsertRedirect(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction)
        {
            var redirect = Build(callHandler, optionsAction);

            return Via.InsertRedirect(redirect);
        }
    }
}
