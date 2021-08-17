using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly Relay<TTarget> _relay;
        private readonly RedirectConfiguration<TTarget> _redirectConfiguration;

        public ViaProxyCall(Relay<TTarget> relay, RedirectConfiguration<TTarget> redirectConfiguration)
        {
            _relay = relay;
            _redirectConfiguration = redirectConfiguration;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relay.CallBegin(_redirectConfiguration, callInfo);
        }
    }
}