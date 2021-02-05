using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateInstanceProxy<T>(T? origin, Func<Diversion<T>?> getDiversion) where T : class
        {
            var interceptor = new DiversionInterceptor<T>(origin, getDiversion);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(origin, interceptor)!;
        }
        
        public T CreateRedirectProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new RedirectInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
        
        public T CreateOriginProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new OriginInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
    }
}