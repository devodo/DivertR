using System;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        protected DelegateRedirectBuilder(IVia<TTarget> via, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(via, callConstraint)
        {
            CallValidator = callValidator ?? throw new ArgumentNullException(nameof(callValidator));
        }

        public IRedirect<TTarget> Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            CallValidator.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var callHandler = new DelegateCallHandler<TTarget>(call => fastDelegate.Invoke(call.Args.InternalArgs));

            return Build(callHandler, optionsAction);
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
