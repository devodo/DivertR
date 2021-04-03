﻿using DivertR.Core;

namespace DivertR.Internal
{
    internal class OriginalProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;

        public OriginalProxyCall(RelayContext<TTarget> relayContext)
        {
            _relayContext = relayContext;
        }
        
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var lastCall = _relayContext.CallInfo;
            var updateCallInfo = new CallInfo<TTarget>(lastCall.ViaProxy, lastCall.Original, callInfo.Method, callInfo.Arguments);
            
            return _relayContext.CallOriginal(updateCallInfo);
        }
    }
}