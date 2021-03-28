using DivertR.Core;

namespace DivertR.Internal
{
    internal class Relay<T> : IRelay<T> where T : class
    {
        private readonly RelayContext<T> _relayContext;
        
        public T Next { get; }
        public T Original { get; }

        public IRedirect<T> Redirect => _relayContext.Redirect;

        public CallInfo<T> CallInfo => _relayContext.CallInfo;

        public Relay(RelayContext<T> relayContext, IProxyFactory proxyFactory)
        {
            _relayContext = relayContext;
            Next = proxyFactory.CreateProxy(new RedirectProxyCall<T>(_relayContext));
            Original = proxyFactory.CreateProxy(new OriginalProxyCall<T>(_relayContext));
        }

        public object? CallNext(CallInfo<T>? callInfo = null)
        {
            return _relayContext.CallNext(callInfo ?? CallInfo);
        }
        
        public object? CallOriginal(CallInfo<T>? callInfo = null)
        {
            return _relayContext.CallOriginal(callInfo ?? CallInfo);
        }
    }
}