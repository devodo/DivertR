using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class RedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;

        private readonly List<Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerChain =
            new List<Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>>>();
        
        private CompositeCallConstraint<TTarget> _callConstraint = CompositeCallConstraint<TTarget>.Empty;

        private int? _orderWeight;
        private bool? _disableSatisfyStrict;

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

        public IRedirectBuilder<TTarget> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink)
        {
            _callHandlerChain.Add(chainLink);

            return this;
        }
        
        public IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }
        
        public IRedirectBuilder<TTarget> DisableSatisfyStrict(bool disableStrict = true)
        {
            _disableSatisfyStrict = disableStrict;

            return this;
        }

        public Redirect<TTarget> Build(TTarget target)
        {
            ICallHandler<TTarget> callHandler = new TargetCallHandler<TTarget>(target);

            return Build(callHandler);
        }

        public IVia<TTarget> Retarget(TTarget target)
        {
            var redirectItem = Build(target);
            
            return InsertRedirect(redirectItem);
        }
        
        protected Redirect<TTarget> Build(ICallHandler<TTarget> callHandler)
        {
            callHandler = ApplyCallHandlerChain(callHandler);

            return new Redirect<TTarget>(callHandler, _callConstraint, _orderWeight, _disableSatisfyStrict);
        }
        
        protected IVia<TTarget> InsertRedirect(Redirect<TTarget> redirect)
        {
            return _via.InsertRedirect(redirect);
        }
        
        private ICallHandler<TTarget> ApplyCallHandlerChain(ICallHandler<TTarget> callHandler)
        {
            foreach (var chainLink in _callHandlerChain)
            {
                callHandler = chainLink.Invoke(_via, callHandler);
            }

            return callHandler;
        }
    }
}
