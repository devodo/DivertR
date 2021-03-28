using System;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class DispatchProxyFactory : IProxyFactory
    {
        public static readonly DispatchProxyFactory Instance = new DispatchProxyFactory();

        public T CreateProxy<T>(T? original, Func<IProxyCall<T>?> getProxyCall) where T : class
        {
            Validate<T>();

            IProxyInvoker CreateProxyInvoker(T proxy)
            {
                return new ProxyWithDefaultInvoker<T>(proxy, original, getProxyCall);
            }
            
            return DiverterDispatchProxy.Create<T>(CreateProxyInvoker);
        }

        public T CreateProxy<T>(IProxyCall<T> proxyCall) where T : class
        {
            Validate<T>();
            
            IProxyInvoker CreateProxyInvoker(T proxy)
            {
                return new ProxyInvoker<T>(proxy, proxyCall);
            }
            
            return DiverterDispatchProxy.Create<T>(CreateProxyInvoker);
        }

        public void Validate<T>()
        {
            if (!typeof(T).IsInterface)
            {
                throw new DiverterException($"Invalid type {typeof(T).Name}. Only interface types are supported");
            }
        }
    }
}