using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        private readonly RedirectState<TTarget> _redirectState;

        public ViaProxyCall(RelayContext<TTarget> relayContext, RedirectState<TTarget> redirectState)
        {
            _relayContext = relayContext;
            _redirectState = redirectState;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relayContext.CallBegin(_redirectState, callInfo);
        }
    }
}