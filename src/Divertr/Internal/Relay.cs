using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class Relay<T> : IRelay<T> where T : class
    {
        private readonly IRelayState<T> _relayState;
        
        public T Next { get; }
        public T Original { get; }

        public IRedirect<T> Redirect => _relayState.Redirect;

        public CallInfo<T> CallInfo => _relayState.CallInfo;

        public Relay(IRelayState<T> relayState, IProxyFactory proxyFactory)
        {
            _relayState = relayState;
            Next = proxyFactory.CreateRedirectTargetProxy(relayState);
            Original = proxyFactory.CreateOriginalTargetProxy(relayState);
        }

        public object? CallNext(CallInfo<T> callInfo)
        {
            return _relayState.CallNext(callInfo);
        }
        
        public object? CallOriginal(CallInfo<T> callInfo)
        {
            return _relayState.CallOriginal(callInfo);
        }
    }
}