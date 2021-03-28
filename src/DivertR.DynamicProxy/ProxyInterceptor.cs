using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class ProxyInterceptor<T> : IInterceptor where T : class
    {
        private readonly IProxyCall<T> _proxyCall;

        public ProxyInterceptor(IProxyCall<T> proxyCall)
        {
            _proxyCall = proxyCall;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var callInfo = new CallInfo<T>((T) invocation.Proxy, null, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = _proxyCall.Call(callInfo);
        }
    }
}