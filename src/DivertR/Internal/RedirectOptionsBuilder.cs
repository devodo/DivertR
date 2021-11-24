using System;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly List<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerChain =
            new List<Func<ICallHandler<TTarget>, ICallHandler<TTarget>>>();

        public RedirectOptionsBuilder(IVia<TTarget> via)
        {
            _via = via;
        }

        public IRedirectOptionsBuilder<TTarget> OrderWeight(int orderWeight)
        {
            _orderWeight = orderWeight;

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> DisableSatisfyStrict(bool disableStrict = true)
        {
            _disableSatisfyStrict = disableStrict;

            return this;
        }

        public IRedirectOptionsBuilder<TTarget> ChainCallHandler(Func<ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink)
        {
            _callHandlerChain.Add(chainLink);

            return this;
        }
        
        public IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount)
        {
            return ChainCallHandler(callHandler => new RepeatCallHandler<TTarget>(_via, callHandler, repeatCount));
        }
        
        public IRedirectOptionsBuilder<TTarget> Skip(int skipCount)
        {
            return ChainCallHandler(callHandler => new SkipCallHandler<TTarget>(_via, callHandler, skipCount));
        }

        public RedirectOptions<TTarget> Build()
        {
            return new RedirectOptions<TTarget>
            {
                OrderWeight = _orderWeight,
                DisableSatisfyStrict = _disableSatisfyStrict,
                CallHandlerDecorator = GetCallHandlerDecorator
            };
        }
        
        private ICallHandler<TTarget> GetCallHandlerDecorator(ICallHandler<TTarget> callHandler)
        {
            if (!_callHandlerChain.Any())
            {
                return callHandler;
            }
            
            foreach (var chainLink in _callHandlerChain)
            {
                callHandler = chainLink.Invoke(callHandler);
            }

            return callHandler;
        }
    }
}