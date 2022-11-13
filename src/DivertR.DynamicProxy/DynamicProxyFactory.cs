using System;
using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class DynamicProxyFactory : IProxyFactory
    {
        private readonly CustomProxyGenerator _proxyGenerator = new();
        private readonly bool _copyRootFields;

        public DynamicProxyFactory(bool copyRootFields = true)
        {
            _copyRootFields = copyRootFields;
        }

        public TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall, TTarget? root = null) where TTarget : class?
        {
            ValidateProxyTarget<TTarget>();
            var interceptor = new ProxyInterceptor<TTarget>(proxyCall, root);

            return CreateProxy(interceptor, root);
        }
        
        public void ValidateProxyTarget<TTarget>()
        {
            if (!(typeof(TTarget).IsInterface || typeof(TTarget).IsClass))
            {
                throw new DiverterException($"Invalid type {typeof(TTarget).Name}. Only interface or class types are supported");
            }
        }

        private TTarget CreateProxy<TTarget>(IInterceptor interceptor, TTarget? root) where TTarget : class?
        {
            if (typeof(TTarget).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget<TTarget>(interceptor);
            }

            if (!typeof(TTarget).IsClass)
            {
                throw new InvalidOperationException("Only interface and class types are supported");
            }

            if (_copyRootFields && root != null)
            {
                return _proxyGenerator.CreateClassProxyWithRootFields(root, interceptor);
            }

            return _proxyGenerator.CreateClassProxy<TTarget>(interceptor);
        }
    }
}