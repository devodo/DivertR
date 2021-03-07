using System;
using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class DynamicProxyFactory : IProxyFactory
    {
        public static readonly DynamicProxyFactory Instance = new DynamicProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateDiverterProxy<T>(T? original, Func<IViaState<T>?> getViaState) where T : class
        {
            var interceptor = new ViaInterceptor<T>(original, getViaState);

            return CreateProxy(interceptor, original);
        }
        
        public T CreateRedirectTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            var interceptor = new RedirectInterceptor<T>(relayState);
            
            return CreateProxy<T>(interceptor);
        }
        
        public T CreateOriginalTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            var interceptor = new OriginInterceptor<T>(relayState);
            
            return CreateProxy<T>(interceptor);
        }

        private T CreateProxy<T>(IInterceptor interceptor, T? target = null) where T : class
        {
            if (typeof(T).IsInterface)
            {
                return target == null
                    ? _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor)
                    : _proxyGenerator.CreateInterfaceProxyWithTarget(target, interceptor);
            }

            if (typeof(T).IsClass)
            {
                return target == null
                    ? _proxyGenerator.CreateClassProxy<T>(interceptor)
                    : _proxyGenerator.CreateClassProxyWithTarget(target, interceptor);
            }
            
            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
    }
}