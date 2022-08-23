namespace DivertR.DispatchProxy
{
    public class DispatchProxyFactory : IProxyFactory
    {
        public TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall, TTarget? root = null) where TTarget : class?
        {
            ValidateProxyTarget<TTarget>();
            
            IProxyInvoker CreateProxyInvoker(TTarget proxy)
            {
                return new ProxyInvoker<TTarget>(proxyCall, proxy!, root);
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