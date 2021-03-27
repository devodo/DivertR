using System.Collections.Generic;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class ViaProxyCall<T> : IProxyCall<T> where T : class
    {
        private readonly IRelayContext<T> _relayContext;
        private readonly IList<IRedirect<T>> _redirects;

        public ViaProxyCall(IRelayContext<T> relayContext, IList<IRedirect<T>> redirects)
        {
            _relayContext = relayContext;
            _redirects = redirects;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            return _relayContext.CallBegin(_redirects, callInfo);
        }
    }
}