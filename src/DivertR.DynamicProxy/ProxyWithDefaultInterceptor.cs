using System;
using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class ProxyWithDefaultInterceptor<TTarget> : IInterceptor where TTarget : class
    {
        private readonly TTarget? _root;
        private readonly Func<IProxyCall?> _getProxyCall;
        
        public ProxyWithDefaultInterceptor(Func<IProxyCall?> getProxyCall)
            : this(null, getProxyCall)
        {
        }

        public ProxyWithDefaultInterceptor(TTarget? root, Func<IProxyCall?> getProxyCall)
        {
            _root = root;
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
            
            var callInfo = new CallInfo<TTarget>((TTarget) invocation.Proxy, _root, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = proxyCall.Call(callInfo);
        }

        private void DefaultProceed(IInvocation invocation)
        {
            if (_root == null)
            {
                throw new DiverterNullRootException("Root instance is null");
            }

            invocation.Proceed();
        }
    }
}