using System;

namespace DivertR
{
    public interface IProxyFactory
    {
        TTarget CreateProxy<TTarget>(Func<IProxyCall<TTarget>?> getProxyCall, TTarget? root = null) where TTarget : class;
        TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall) where TTarget : class;
        void ValidateProxyTarget<TTarget>();
    }
}