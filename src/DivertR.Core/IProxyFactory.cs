using System;

namespace DivertR.Core
{
    public interface IProxyFactory
    {
        TTarget CreateProxy<TTarget>(TTarget? original, Func<IProxyCall<TTarget>?> getProxyCall) where TTarget : class;
        TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall) where TTarget : class;
        void ValidateProxyTarget<TTarget>();
    }
}