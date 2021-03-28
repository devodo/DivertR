using System;

namespace DivertR.Core
{
    public interface IProxyFactory
    {
        T CreateProxy<T>(T? original, Func<IProxyCall<T>?> getProxyCall) where T : class;
        T CreateProxy<T>(IProxyCall<T> proxyCall) where T : class;
        void Validate<T>();
    }
}