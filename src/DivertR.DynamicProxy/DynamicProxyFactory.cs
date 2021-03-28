using System;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    internal class DynamicProxyFactory : IProxyFactory
    {
        public static readonly DynamicProxyFactory Instance = new DynamicProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public T CreateProxy<T>(T? original, Func<IProxyCall<T>?> getProxyCall) where T : class
        {
            Validate<T>();
            var interceptor = new ProxyWithDefaultInterceptor<T>(original, getProxyCall);

            return CreateProxy(interceptor, original);
        }

        public T CreateProxy<T>(IProxyCall<T> proxyCall) where T : class
        {
            Validate<T>();
            var interceptor = new ProxyInterceptor<T>(proxyCall);

            return CreateProxy<T>(interceptor);
        }

        public void Validate<T>()
        {
            if (!(typeof(T).IsInterface || typeof(T).IsClass))
            {
                throw new DiverterException($"Invalid type {typeof(T).Name}. Only interface or class types are supported");
            }
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

            throw new NotImplementedException();
        }
    }
}