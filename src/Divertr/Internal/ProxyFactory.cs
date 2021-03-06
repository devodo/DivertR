using System;
using DivertR.Internal.DynamicProxy;

namespace DivertR.Internal
{
    internal class ProxyFactory : IProxyFactory
    {
        public static readonly IProxyFactory Instance = new ProxyFactory(new DynamicProxyFactory());
        
        private readonly IProxyFactory _proxyFactory;

        private ProxyFactory(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }
        public T CreateDiverterProxy<T>(T? original, Func<ViaWay<T>?> getViaWay) where T : class
        {
            return _proxyFactory.CreateDiverterProxy(original, getViaWay);
        }

        public T CreateRedirectTargetProxy<T>(Relay<T> relay) where T : class
        {
            return _proxyFactory.CreateRedirectTargetProxy(relay);
        }

        public T CreateOriginalTargetProxy<T>(Relay<T> relay) where T : class
        {
            return _proxyFactory.CreateOriginalTargetProxy(relay);
        }
    }
}