using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class ProxyInterceptor<TTarget> : IInterceptor where TTarget : class?
    {
        private readonly IProxyCall<TTarget> _proxyCall;
        private readonly TTarget? _root;

        public ProxyInterceptor(IProxyCall<TTarget> proxyCall, TTarget? root)
        {
            _proxyCall = proxyCall;
            _root = root;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var callReturn = _proxyCall.Call((TTarget) invocation.Proxy, _root, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = callReturn;
        }
    }
}