using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal abstract class DelegateRedirectBuilder<TTarget> : IDelegateRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly ParsedCallExpression _parsedCallExpression;
        
        protected CompositeCallConstraint<TTarget> CompositeCallConstraint;
        protected int OrderWeight = 0;

        protected DelegateRedirectBuilder(IVia<TTarget> via, ParsedCallExpression parsedCallExpression)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));
            _parsedCallExpression = parsedCallExpression ?? throw new ArgumentNullException(nameof(parsedCallExpression));
            CompositeCallConstraint = CompositeCallConstraint<TTarget>.Empty.AddCallConstraint(_parsedCallExpression.ToCallConstraint<TTarget>());
        }

        public ICallConstraint<TTarget> CallConstraint => CompositeCallConstraint;

        public IRedirect<TTarget> Build(TTarget target)
        {
            return new TargetRedirect<TTarget>(target, CallConstraint);
        }

        public IRedirect<TTarget> Build(Delegate redirectDelegate)
        {
            _parsedCallExpression.Validate(redirectDelegate);
            var fastDelegate = redirectDelegate.ToDelegate();
            
            return new DelegateRedirect<TTarget>(callInfo => fastDelegate.Invoke(callInfo.Arguments.InternalArgs), CallConstraint);
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
            _parsedCallExpression.Validate(inputDelegate);
            
            return new DelegateRedirect<TTarget>(mappedRedirect, CallConstraint);
        }

        protected IVia<TTarget> InsertRedirect(Delegate inputDelegate, Func<CallInfo<TTarget>, object?> mappedRedirect)
        {
            var redirect = Build(inputDelegate, mappedRedirect);

            return _via.InsertRedirect(redirect, OrderWeight);
        }
    }
}