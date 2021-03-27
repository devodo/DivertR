using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class RedirectProxyCall<T> : IProxyCall<T> where T : class
    {
        private readonly IRelayContext<T> _relayContext;

        public RedirectProxyCall(IRelayContext<T> relayContext)
        {
            _relayContext = relayContext;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            var lastCall = _relayContext.CallInfo;
            var updateCallInfo = new CallInfo<T>(lastCall.Proxy, lastCall.Original, callInfo.Method, callInfo.CallArguments);
            
            return _relayContext.CallNext(updateCallInfo);
        }
    }
}