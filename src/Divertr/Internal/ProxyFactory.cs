using System;
using DivertR.Core.Internal;
using DivertR.DynamicProxy;

namespace DivertR.Internal
{
    internal class ProxyFactory2 : IProxyFactory
    {
        public static readonly IProxyFactory Instance = new ProxyFactory2(new DynamicProxyFactory());
        
        private readonly IProxyFactory _proxyFactory;

        private ProxyFactory2(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }
        
        public T CreateDiverterProxy<T>(T? original, Func<IViaState<T>?> getViaState) where T : class
        {
            return _proxyFactory.CreateDiverterProxy(original, getViaState);
        }

        public T CreateRedirectTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            return _proxyFactory.CreateRedirectTargetProxy(relayState);
        }

        public T CreateOriginalTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            return _proxyFactory.CreateOriginalTargetProxy(relayState);
        }
    }
}