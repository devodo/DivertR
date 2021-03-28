using DivertR.Core;

namespace DivertR.Internal
{
    internal class OriginalProxyCall<T> : IProxyCall<T> where T : class
    {
        private readonly RelayContext<T> _relayContext;

        public OriginalProxyCall(RelayContext<T> relayContext)
        {
            _relayContext = relayContext;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            var lastCall = _relayContext.CallInfo;
            var updateCallInfo = new CallInfo<T>(lastCall.ViaProxy, lastCall.Original, callInfo.Method, callInfo.Arguments);
            
            return _relayContext.CallOriginal(updateCallInfo);
        }
    }
}