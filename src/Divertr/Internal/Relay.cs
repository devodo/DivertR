using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class Relay<T> : IRelay<T> where T : class
    {
        private readonly IRelayState<T> _relayState;
        
        public T Next { get; }
        public T Original { get; }
        public T? OriginalInstance => _relayState.Original;
        public IRedirect<T> Redirect => _relayState.Redirect;

        public Relay(IRelayState<T> relayState, IProxyFactory proxyFactory)
        {
            _relayState = relayState;
            Next = proxyFactory.CreateRedirectTargetProxy(relayState);
            Original = proxyFactory.CreateOriginalTargetProxy(relayState);
        }

        public object? CallNext(ICall call)
        {
            return _relayState.CallNext(call);
        }
        
        public object? CallOriginal(ICall call)
        {
            return _relayState.CallOriginal(call);
        }
    }
}