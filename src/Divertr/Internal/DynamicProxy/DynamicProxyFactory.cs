using System;
using Castle.DynamicProxy;

namespace DivertR.Internal.DynamicProxy
{
    internal class DynamicProxyFactory : IProxyFactory
    {
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiverterProxy<T>(T? original, Func<ViaWay<T>?> getViaWay) where T : class
        {
            var interceptor = new ViaInterceptor<T>(original, getViaWay);

            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(original, interceptor)!;
            }

            if (typeof(T).IsClass)
            {
                return _proxyGenerator.CreateClassProxyWithTarget(original, interceptor)!;
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
        
        public T CreateRedirectTargetProxy<T>(Relay<T> relay) where T : class
        {
            var interceptor = new RedirectInterceptor<T>(relay);
            
            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
            }

            if (typeof(T).IsClass)
            {
                return _proxyGenerator.CreateClassProxy<T>(interceptor)!;
            }
            
            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
        
        public T CreateOriginalTargetProxy<T>(Relay<T> relay) where T : class
        {
            var interceptor = new OriginInterceptor<T>(relay);
            
            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null!, interceptor);
            }

            if (typeof(T).IsClass)
            {
                return _proxyGenerator.CreateClassProxy<T>(interceptor)!;
            }
            
            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
    }
}