using System;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        protected DelegateRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
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
    }
}
