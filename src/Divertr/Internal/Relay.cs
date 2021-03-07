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

        public object? State => _relayState.Redirect.State;

        public Relay(IRelayState<T> relayState, IProxyFactory proxyFactory)
        {
            _relayState = relayState;
            Next = proxyFactory.CreateRedirectTargetProxy(relayState);
            Original = proxyFactory.CreateOriginalTargetProxy(relayState);
        }
    }
}