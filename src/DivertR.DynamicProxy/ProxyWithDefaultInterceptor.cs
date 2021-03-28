using System;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class ProxyWithDefaultInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<IProxyCall<T>?> _getProxyCall;
        
        public ProxyWithDefaultInterceptor(Func<IProxyCall<T>?> getProxyCall)
            : this(null, getProxyCall)
        {
        }

        public ProxyWithDefaultInterceptor(T? original, Func<IProxyCall<T>?> getProxyCall)
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
            
            var callInfo = new CallInfo<T>((T) invocation.Proxy, _original, invocation.Method, invocation.Arguments);
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