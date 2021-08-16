﻿using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class OriginalProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;

        public OriginalProxyCall(RelayContext<TTarget> relayContext)
        {
            _relayContext = relayContext;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relayContext.CallOriginal(callInfo.Method, callInfo.Arguments);
        }
    }
}