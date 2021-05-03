using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly List<Func<IRedirect<TTarget>, IRedirect<TTarget>>> _redirectDecorators = new List<Func<IRedirect<TTarget>, IRedirect<TTarget>>>();
        private CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;
        private int _orderWeight;

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

        public IRedirectBuilder<TTarget> AddRedirectDecorator(Func<IRedirect<TTarget>, IRedirect<TTarget>> decorator)
        {
            _redirectDecorators.Add(decorator);

            return this;
        }

        public ICallConstraint<TTarget> CallConstraint => _callConstraint;

        public IRedirect<TTarget> Build(TTarget target)
        {
            var redirect = new TargetRedirect<TTarget>(target, _callConstraint);

            return Decorate(redirect);
        }
        
        public IVia<TTarget> To(TTarget target)
        {
            var redirect = Build(target);
            
            return _via.InsertRedirect(redirect, _orderWeight);
        }
        
        protected IVia<TTarget> InsertRedirect(IRedirect<TTarget> redirect)
        {
            return _via.InsertRedirect(redirect, _orderWeight);
        }
        
        protected IRedirect<TTarget> Decorate(IRedirect<TTarget> redirect)
        {
            foreach (var decorator in _redirectDecorators)
            {
                redirect = decorator.Invoke(redirect);
            }

            return redirect;
        }
    }
}
