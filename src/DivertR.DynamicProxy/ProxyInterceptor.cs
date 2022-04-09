using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class ProxyInterceptor<TTarget> : IInterceptor where TTarget : class
    {
        private readonly IProxyCall _proxyCall;

        public ProxyInterceptor(IProxyCall proxyCall)
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