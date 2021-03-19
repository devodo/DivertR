﻿using System.Reflection;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class RedirectInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly IRelayState<T> _relayState;

        public RedirectInvoker(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }
        
        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var call = new DispatchProxyCall(targetMethod, args);

            return _relayState.InvokeNext(call)!;
        }
    }
}