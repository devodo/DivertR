using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class NextProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;

        public NextProxyCall(RelayContext<TTarget> relayContext)
        {
            _relayContext = relayContext;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relayContext.CallNext(callInfo.Method, callInfo.Arguments);
        }
    }
}