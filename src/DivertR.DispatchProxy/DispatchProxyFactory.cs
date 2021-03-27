using System;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class DispatchProxyFactory : IProxyFactory
    {
        public static readonly DispatchProxyFactory Instance = new DispatchProxyFactory();

        public T CreateProxy<T>(T? original, Func<IProxyCall<T>?> getProxyCall) where T : class
        {
            return CreateProxy<T>(proxy => new ProxyWithDefaultInvoker<T>(proxy, original, getProxyCall));
        }

        public T CreateProxy<T>(IProxyCall<T> proxyCall) where T : class
        {
            return CreateProxy<T>(proxy => new ProxyInvoker<T>(proxy, proxyCall));
        }
        
        private static T CreateProxy<T>(Func<T, IDispatchProxyInvoker> invokerFactory) where T : class
        {
            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invokerFactory);
            }

            throw new DiverterException($"Invalid type {typeof(T).Name}. Only interface types are supported");
        }
    }
}