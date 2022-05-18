using System;

namespace DivertR.Internal
{
    internal abstract class DelegateViaBuilder<TTarget> : ViaBuilder<TTarget>, IDelegateViaBuilder<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        protected DelegateViaBuilder(IRedirectRepository redirectRepository, ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(redirectRepository, callConstraint)
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

        public IDelegateViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            RedirectRepository.InsertRedirect(redirect);

            return this;
        }

        protected void InsertRedirect(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction)
        {
            var redirect = Build(callHandler, optionsAction);
            RedirectRepository.InsertRedirect(redirect);
        }
    }
}
