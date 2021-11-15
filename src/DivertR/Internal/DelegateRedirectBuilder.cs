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

        public IVia<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
        {
            var redirect = Build(redirectDelegate, optionsAction);
            
            return InsertRedirect(redirect);
        }
        
        protected Redirect<TTarget> Build(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            ParsedCallExpression.Validate(inputDelegate);
            var redirect = new DelegateCallHandler<TTarget>(mappedRedirect);

            return Build(redirect);
        }

        protected IVia<TTarget> InsertRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return InsertRedirect(redirect);
        }
        
        protected IVia<TTarget> InsertRedirect(Func<CallInfo<TTarget>, object?> mappedRedirect, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction)
        {
            var callHandler = new DelegateCallHandler<TTarget>(mappedRedirect);
            var redirect = Build(callHandler, optionsAction);

            return InsertRedirect(redirect);
        }
        
        protected IVia<TTarget> InsertRedirect(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction)
        {
            var redirect = Build(callHandler, optionsAction);

            return InsertRedirect(redirect);
        }
    }
}
