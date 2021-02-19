using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiverterProxy<T>(T? original, Func<RedirectRoute<T>?> getRedirectRoute) where T : class
        {
            var interceptor = new DiverterInterceptor<T>(original, getRedirectRoute);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(original, interceptor)!;
        }
        
        public T CreateRedirectTargetProxy<T>(CallRelay<T> callRelay) where T : class
        {
            var interceptor = new RedirectTargetInterceptor<T>(callRelay);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
        
        public T CreateOriginalTargetProxy<T>(CallRelay<T> callRelay) where T : class
        {
            var interceptor = new OriginalTargetInterceptor<T>(callRelay);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
    }
}