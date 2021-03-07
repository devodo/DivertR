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

        public Relay(IRelayState<T> relayState)
        {
            _relayState = relayState;
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(relayState);
            Original = ProxyFactory.Instance.CreateOriginalTargetProxy(relayState);
        }
    }
}