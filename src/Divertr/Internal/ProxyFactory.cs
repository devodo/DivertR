using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiversionProxy<T>(T? original, Func<DiversionSnapshot<T>?> getDirector) where T : class
        {
            var interceptor = new DiversionInterceptor<T>(original, getDirector);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(original, interceptor)!;
        }
        
        public T CreateRedirectTargetProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new RedirectTargetInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
        
        public T CreateRootTargetProxy<T>(CallContext<T> callContext) where T : class
        {
            var interceptor = new RootTargetInterceptor<T>(callContext);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
        }
    }
}