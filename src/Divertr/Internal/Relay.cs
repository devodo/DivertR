using DivertR.Core;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        
        public TTarget Next { get; }
        public TTarget Original { get; }

        public IRedirect<TTarget> Redirect => _relayContext.Redirect;

        public CallInfo<TTarget> CallInfo => _relayContext.CallInfo;

        public Relay(RelayContext<TTarget> relayContext, IProxyFactory proxyFactory)
        {
            _relayContext = relayContext;
            Next = proxyFactory.CreateProxy(new RedirectProxyCall<TTarget>(_relayContext));
            Original = proxyFactory.CreateProxy(new OriginalProxyCall<TTarget>(_relayContext));
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