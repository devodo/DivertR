﻿using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;

        public RedirectProxyCall(RelayContext<TTarget> relayContext)
        {
            _relayContext = relayContext;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var lastCall = _relayContext.CallInfo;
            var updateCallInfo = new CallInfo<TTarget>(lastCall.ViaProxy, lastCall.Original, callInfo.Method, callInfo.Arguments);
            
            return _relayContext.CallNext(updateCallInfo);
        }
    }
}