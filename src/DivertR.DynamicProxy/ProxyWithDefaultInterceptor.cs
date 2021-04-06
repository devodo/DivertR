using System;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    public class ProxyWithDefaultInterceptor<TTarget> : IInterceptor where TTarget : class
    {
        private readonly TTarget? _original;
        private readonly Func<IProxyCall<TTarget>?> _getProxyCall;
        
        public ProxyWithDefaultInterceptor(Func<IProxyCall<TTarget>?> getProxyCall)
            : this(null, getProxyCall)
        {
        }

        public ProxyWithDefaultInterceptor(TTarget? original, Func<IProxyCall<TTarget>?> getProxyCall)
        {
            _original = original;
            _getProxyCall = getProxyCall;
        }

        public void Intercept(IInvocation invocation)
        {
            var proxyCall = _getProxyCall();

            if (proxyCall == null)
            {
                DefaultProceed(invocation);
                return;
            }
            
            var callInfo = new CallInfo<TTarget>((TTarget) invocation.Proxy, _original, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = proxyCall.Call(callInfo);
        }

        private void DefaultProceed(IInvocation invocation)
        {
            if (_original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }

            invocation.Proceed();
        }
    }
}