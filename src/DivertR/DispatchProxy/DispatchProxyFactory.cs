using System;

namespace DivertR.DispatchProxy
{
    public class DispatchProxyFactory : IProxyFactory
    {
        public TTarget CreateProxy<TTarget>(Func<IProxyCall?> getProxyCall, TTarget? root = null) where TTarget : class
        {
            ValidateProxyTarget<TTarget>();

            IProxyInvoker CreateProxyInvoker(TTarget proxy)
            {
                return new ProxyWithDefaultInvoker<TTarget>(proxy, root, getProxyCall);
            }
            
            return DiverterDispatchProxy.Create<TTarget>(CreateProxyInvoker);
        }

        public TTarget CreateProxy<TTarget>(IProxyCall proxyCall) where TTarget : class
        {
            ValidateProxyTarget<TTarget>();
            
            IProxyInvoker CreateProxyInvoker(TTarget proxy)
            {
                return new ProxyInvoker<TTarget>(proxy, proxyCall);
            }
            
            return DiverterDispatchProxy.Create<TTarget>(CreateProxyInvoker);
        }

        public void ValidateProxyTarget<TTarget>()
        {
            if (!typeof(TTarget).IsInterface)
            {
                throw new DiverterException($"Invalid type {typeof(TTarget).Name}. Only interface types are supported");
            }
        }
    }
}