using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateInstanceProxy<T>(T origin, Func<Diversion<T>> getDiversion) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new DiversionInterceptor<T>(origin, getDiversion);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(origin, interceptor);
        }
        
        public T CreateRedirectProxy<T>(CallContext<T> callContext) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new RedirectInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null, interceptor);
        }
    }
}