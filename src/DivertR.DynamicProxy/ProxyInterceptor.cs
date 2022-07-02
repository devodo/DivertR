using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class ProxyInterceptor<TTarget> : IInterceptor where TTarget : class
    {
        private readonly IProxyCall<TTarget> _proxyCall;

        public ProxyInterceptor(IProxyCall<TTarget> proxyCall)
        {
            _proxyCall = proxyCall;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var callInfo = CallInfoFactory.Create((TTarget) invocation.Proxy, null, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = _proxyCall.Call(callInfo);
        }
    }
}