using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class DynamicProxyFactory : IProxyFactory
    {
        private readonly ProxyGenerator _proxyGenerator = new();
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

            if (typeof(TTarget).IsClass)
            {
                var proxy = _proxyGenerator.CreateClassProxy<TTarget>(interceptor);

                if (_copyRootFields && root != null)
                {
                    CopyFields(root, proxy);
                }

                return proxy;
            }

            throw new NotImplementedException();
        }

        private static void CopyFields<TTarget>(TTarget src, TTarget dest)
        {
            const BindingFlags FieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in typeof(TTarget).GetFields(FieldFlags))
            {
                var fieldValue = field.GetValue(src);
                field.SetValue(dest, fieldValue);
            }
        }
    }
}