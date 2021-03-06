using System;
using Castle.DynamicProxy;

namespace DivertR.Internal.DispatchProxy
{
    internal class DispatchProxyFactory : IProxyFactory
    {
        public T CreateDiverterProxy<T>(T? original, Func<ViaWay<T>?> getViaWay) where T : class
        {
            var invoker = new ViaInvoker<T>(original, getViaWay);

            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface types are supported");
        }
        
        public T CreateRedirectTargetProxy<T>(Relay<T> relay) where T : class
        {
            var invoker = new RedirectInvoker<T>(relay);
            
            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface types are supported");
        }
        
        public T CreateOriginalTargetProxy<T>(Relay<T> relay) where T : class
        {
            var invoker = new OriginInvoker<T>(relay);
            
            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
    }
}