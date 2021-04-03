﻿using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class ProxyInterceptor<TTarget> : IInterceptor where TTarget : class
    {
        private readonly IProxyCall<TTarget> _proxyCall;

        public ProxyInterceptor(IProxyCall<TTarget> proxyCall)
        {
            _proxyCall = proxyCall;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var callInfo = new CallInfo<TTarget>((TTarget) invocation.Proxy, null, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = _proxyCall.Call(callInfo);
        }
    }
}