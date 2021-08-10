using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _original;

        public TTarget Next => _next.Value;
        public TTarget Original => _original.Value;

        public IRedirect<TTarget> Redirect => _relayContext.Redirect;

        public CallInfo<TTarget> CallInfo => _relayContext.CallInfo;

        public Relay(RelayContext<TTarget> relayContext, IProxyFactory proxyFactory)
        {
            _relayContext = relayContext;
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new RedirectProxyCall<TTarget>(_relayContext)));
            _original = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new OriginalProxyCall<TTarget>(_relayContext)));
        }

        public object? CallNext(CallInfo<TTarget>? callInfo = null)
        {
            return _relayContext.CallNext(callInfo ?? CallInfo);
        }
        
        public object? CallOriginal(CallInfo<TTarget>? callInfo = null)
        {
            return _relayContext.CallOriginal(callInfo ?? CallInfo);
        }
    }
}