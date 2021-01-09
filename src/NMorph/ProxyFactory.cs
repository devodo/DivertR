using System;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class ProxyFactory
    {
        public static readonly ProxyFactory Instance = new ProxyFactory();
        
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        
        public T CreateMorphProxy<T>(T origin, Func<Alteration<T>> getAlteration) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new MorphInterceptor<T>(origin, getAlteration);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface(origin, interceptor);
        }
        
        public T CreateSubstitutionProxy<T>(InvocationStack<T> invocationStack) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            var interceptor = new SubstitutionInterceptor<T>(invocationStack);
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(null, interceptor);
        }
    }
}