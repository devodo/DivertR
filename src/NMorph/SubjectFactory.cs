using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class SubjectFactory
    {
        public static readonly SubjectFactory Instance = new SubjectFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T Create<T>(AlterationStore alterationStore, T origin = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new MorphInterceptor<T>(alterationStore, origin);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(origin, interceptor);
        }
    }
}