using System;
using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class DynamicProxyFactory : IProxyFactory
    {
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiverterProxy<T>(T? original, Func<IViaState<T>?> getViaState) where T : class
        {
            var interceptor = new ViaInterceptor<T>(original, getViaState);

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
        
        public T CreateRedirectTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            var interceptor = new RedirectInterceptor<T>(relayState);
            
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
        
        public T CreateOriginalTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            var interceptor = new OriginInterceptor<T>(relayState);
            
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