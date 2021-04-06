using System;
using Castle.DynamicProxy;
using DivertR.Core;

namespace DivertR.DynamicProxy
{
    public class DynamicProxyFactory : IProxyFactory
    {
        public static readonly DynamicProxyFactory Instance = new DynamicProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public TTarget CreateProxy<TTarget>(TTarget? original, Func<IProxyCall<TTarget>?> getProxyCall) where TTarget : class
        {
            ValidateProxyTarget<TTarget>();
            var interceptor = new ProxyWithDefaultInterceptor<TTarget>(original, getProxyCall);

            return CreateProxy(interceptor, original);
        }

        public TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall) where TTarget : class
        {
            ValidateProxyTarget<TTarget>();
            var interceptor = new ProxyInterceptor<TTarget>(proxyCall);

            return CreateProxy<TTarget>(interceptor);
        }

        public void ValidateProxyTarget<TTarget>()
        {
            if (!(typeof(TTarget).IsInterface || typeof(TTarget).IsClass))
            {
                throw new DiverterException($"Invalid type {typeof(TTarget).Name}. Only interface or class types are supported");
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