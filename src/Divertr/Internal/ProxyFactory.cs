using System;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiverterProxy<T>(T? original, Func<ViaWay<T>?> getRedirectRoute) where T : class
        {
            var interceptor = new ViaInterceptor<T>(original, getRedirectRoute);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(original, interceptor)!;
        }
        
        public T CreateRedirectTargetProxy<T>(Relay<T> relay) where T : class
        {
            var interceptor = new RedirectInterceptor<T>(relay);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
        
        public T CreateOriginalTargetProxy<T>(Relay<T> relay) where T : class
        {
            var interceptor = new OriginInterceptor<T>(relay);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
    }
}