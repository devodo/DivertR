using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        protected CompositeCallConstraint<TTarget> CompositeCallConstraint;
        protected readonly ParsedCallExpression ParsedCallExpression;

        public DelegateRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            ParsedCallExpression = parsedCallExpression ?? throw new ArgumentNullException(nameof(parsedCallExpression));
            CompositeCallConstraint = CompositeCallConstraint<TTarget>.Empty.AddCallConstraint(ParsedCallExpression.ToCallConstraint<TTarget>());
        }

        public ICallConstraint<TTarget> CallConstraint => CompositeCallConstraint;

        public IRedirect<TTarget> Build(TTarget target)
        {
            return new TargetRedirect<TTarget>(target, CallConstraint);
        }

        public virtual IRedirect<TTarget> Build(Delegate redirectDelegate)
        {
            var fastDelegate = redirectDelegate.Method.ToDelegate(redirectDelegate.GetType());
            return new DelegateRedirect<TTarget>(callInfo => fastDelegate.Invoke(redirectDelegate, callInfo.Arguments.InternalArgs), CallConstraint);
            //return new DelegateRedirect<T>(callInfo => redirectDelegate.DynamicInvoke(callInfo.Arguments.InternalArgs), BuildCallConstraint());
        }

        public IVia<TTarget> To(TTarget target)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect);
        }
        
        public IVia<TTarget> To(Delegate redirectDelegate)
        {
            var redirect = Build(redirectDelegate);
            
            return _via.InsertRedirect(redirect);
        }
        
        protected IRedirect<TTarget> Build(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            ParsedCallExpression.Validate(inputDelegate);
            
            return new DelegateRedirect<TTarget>(mappedRedirect, CallConstraint);
        }

        protected IVia<TTarget> InsertRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return _via.InsertRedirect(redirect);
        }
    }
}