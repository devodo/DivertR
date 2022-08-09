using System;
using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class DynamicProxyFactory : IProxyFactory
    {
        public static readonly DynamicProxyFactory Instance = new DynamicProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall, TTarget? root = null) where TTarget : class?
        {
            ValidateProxyTarget<TTarget>();
            var interceptor = new ProxyInterceptor<TTarget>(proxyCall, root);

            return CreateProxy<TTarget>(interceptor);
        }
        

        public void ValidateProxyTarget<TTarget>()
        {
            if (!(typeof(TTarget).IsInterface || typeof(TTarget).IsClass))
            {
                throw new DiverterException($"Invalid type {typeof(TTarget).Name}. Only interface or class types are supported");
            }
        }

        private TTarget CreateProxy<TTarget>(IInterceptor interceptor) where TTarget : class?
        {
            if (typeof(TTarget).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget<TTarget>(interceptor);
            }

            if (typeof(TTarget).IsClass)
            {
                return _proxyGenerator.CreateClassProxy<TTarget>(interceptor);
            }

            throw new NotImplementedException();
        }
    }
}