using System;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;
        private int _orderWeight = 0;

        public RedirectBuilder(IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
        {
            _via = via ?? throw new ArgumentNullException(nameof(via));

            if (callConstraint != null)
            {
                _callConstraint = _callConstraint.AddCallConstraint(callConstraint);
            }
        }

        public IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint)
        {
            _callConstraint = _callConstraint.AddCallConstraint(callConstraint);

            return this;
        }

        public IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }

        public ICallConstraint<TTarget> CallConstraint => _callConstraint;

        public IRedirect<TTarget> Build(TTarget target)
        {
            return new TargetRedirect<TTarget>(target, _callConstraint);
        }
        
        public IVia<TTarget> To(TTarget target)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect, _orderWeight);
        }
    }
}