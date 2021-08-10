using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        private readonly IList<IRedirect<TTarget>> _redirects;

        public ViaProxyCall(RelayContext<TTarget> relayContext, IList<IRedirect<TTarget>> redirects)
        {
            _relayContext = relayContext;
            _redirects = redirects;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relayContext.CallBegin(_redirects, callInfo);
        }
    }
}