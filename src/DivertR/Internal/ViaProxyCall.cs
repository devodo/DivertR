using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        private readonly RedirectConfiguration<TTarget> _redirectConfiguration;

        public ViaProxyCall(RelayContext<TTarget> relayContext, RedirectConfiguration<TTarget> redirectConfiguration)
        {
            _relayContext = relayContext;
            _redirectConfiguration = redirectConfiguration;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relayContext.CallBegin(_redirectConfiguration, callInfo);
        }
    }
}