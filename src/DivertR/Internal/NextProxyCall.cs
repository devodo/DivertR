﻿using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class NextProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly IRelay _relay;

        public NextProxyCall(IRelay relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(ICallInfo<TTarget> callInfo)
        {
            return _relay.CallNext(callInfo.Method, callInfo.Arguments);
        }
    }
}