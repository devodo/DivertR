using System;

namespace DivertR
{
    public interface IProxyFactory
    {
        TTarget CreateProxy<TTarget>(Func<IProxyCall?> getProxyCall, TTarget? root = null) where TTarget : class;
        TTarget CreateProxy<TTarget>(IProxyCall proxyCall) where TTarget : class;
        void ValidateProxyTarget<TTarget>();
    }
}