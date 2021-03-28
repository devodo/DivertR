using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaProxyCall<T> : IProxyCall<T> where T : class
    {
        private readonly RelayContext<T> _relayContext;
        private readonly IList<IRedirect<T>> _redirects;

        public ViaProxyCall(RelayContext<T> relayContext, IList<IRedirect<T>> redirects)
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