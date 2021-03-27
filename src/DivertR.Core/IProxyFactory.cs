using System;
using DivertR.Core.Internal;

namespace DivertR.Core
{
    public interface IProxyFactory
    {
        T CreateProxy<T>(T? original, Func<IProxyCall<T>?> getProxyCall) where T : class;
        T CreateProxy<T>(IProxyCall<T> proxyCall) where T : class;
    }
}