using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateInstanceProxy<T>(T? original, Func<CallRoute<T>?> getDirector) where T : class
        {
            var interceptor = new DiverterInterceptor<T>(original, getDirector);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(original, interceptor)!;
        }
        
        public T CreateReplacedProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new ReplacedInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
        
        public T CreateOriginalProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new OriginalInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
    }
}