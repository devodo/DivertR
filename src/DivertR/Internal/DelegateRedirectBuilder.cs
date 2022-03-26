using System;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ParsedCallExpression ParsedCallExpression;

        protected DelegateRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression, ICallConstraint callConstraint)
            : base(via, callConstraint)
        {
            ParsedCallExpression = parsedCallExpression ?? throw new ArgumentNullException(nameof(parsedCallExpression));
        }

        public Redirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            ParsedCallExpression.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var redirect = new DelegateCallHandler<TTarget>(callInfo => fastDelegate.Invoke(callInfo.Arguments.InternalArgs));

            return Build(redirect, optionsAction);
        }

        public IDelegateRedirectBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            Via.InsertRedirect(redirect);

            return this;
        }

        protected IVia<TTarget> InsertRedirect(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction)
        {
            var redirect = Build(callHandler, optionsAction);

            return Via.InsertRedirect(redirect);
        }
    }
}
