using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.Extensions;

namespace DivertR.Internal
{
    internal class RedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        private int? _orderWeight;
        private bool? _disableSatisfyStrict;
        
        private readonly List<Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>>> _callHandlerChain =
            new List<Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>>>();

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

        public IRedirectOptionsBuilder<TTarget> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink)
        {
            _callHandlerChain.Add(chainLink);

            return this;
        }
        
        public IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount)
        {
            return ChainCallHandler((via, redirect) => new RepeatCallHandler<TTarget>(via, redirect, repeatCount));
        }
        
        public IRedirectOptionsBuilder<TTarget> Skip(int skipCount)
        {
            return ChainCallHandler((via, redirect) => new SkipCallHandler<TTarget>(via, redirect, skipCount));
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
        
        private ICallHandler<TTarget> GetCallHandlerDecorator(IVia<TTarget> via, ICallHandler<TTarget> callHandler)
        {
            if (!_callHandlerChain.Any())
            {
                return callHandler;
            }
            
            foreach (var chainLink in _callHandlerChain)
            {
                callHandler = chainLink.Invoke(via, callHandler);
            }

            return callHandler;
        }
    }
}