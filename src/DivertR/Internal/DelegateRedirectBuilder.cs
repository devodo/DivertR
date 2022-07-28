using System;
using System.Collections.Concurrent;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : RedirectBuilder<TTarget>, IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        protected DelegateRedirectBuilder(ICallValidator callValidator, ICallConstraint<TTarget> callConstraint)
            : base(callConstraint)
        {
            CallValidator = callValidator;
        }
        
        protected DelegateRedirectBuilder(ICallValidator callValidator, ConcurrentBag<ICallConstraint<TTarget>> callConstraints)
            : base(callConstraints)
        {
            CallValidator = callValidator;
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
        private readonly ICallValidator _callValidator;

        protected DelegateRedirectBuilder(ICallValidator callValidator, ICallConstraint callConstraint)
            : base(callConstraint)
        {
            _callValidator = callValidator;
        }
        
        protected DelegateRedirectBuilder(ICallValidator callValidator, ConcurrentBag<ICallConstraint> callConstraints)
            : base(callConstraints)
        {
            _callValidator = callValidator;
        }

        public IRedirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            _callValidator.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            var callHandler = new CallHandler(call => fastDelegate.Invoke(call.Args.InternalArgs));

            return Build(callHandler, optionsAction);
        }
    }
}
