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

        public IRedirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            CallValidator.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var callHandler = new CallHandler<TTarget>(call => fastDelegate.Invoke(call.Args.InternalArgs));

            return Build(callHandler, optionsAction);
        }
    }
    
    internal abstract class DelegateRedirectBuilder : RedirectBuilder, IDelegateRedirectBuilder
    {
        protected readonly ICallValidator CallValidator;

        protected DelegateRedirectBuilder(ICallValidator callValidator, ICallConstraint callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator ?? throw new ArgumentNullException(nameof(callValidator));
        }

        public IRedirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            CallValidator.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var callHandler = new CallHandler(call => fastDelegate.Invoke(call.Args.InternalArgs));

            return Build(callHandler, optionsAction);
        }
    }
}
