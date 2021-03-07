using System;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class DispatchProxyFactory : IProxyFactory
    {
        public T CreateDiverterProxy<T>(T? original, Func<IViaState<T>?> getViaState) where T : class
        {
            var invoker = new ViaInvoker<T>(original, getViaState);

            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface types are supported");
        }
        
        public T CreateRedirectTargetProxy<T>(IRelayState<T> relay) where T : class
        {
            var invoker = new RedirectInvoker<T>(relay);
            
            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface types are supported");
        }
        
        public T CreateOriginalTargetProxy<T>(IRelayState<T> relayState) where T : class
        {
            var invoker = new OriginInvoker<T>(relayState);
            
            if (typeof(T).IsInterface)
            {
                return DiverterProxy.Create<T>(invoker);
            }

            throw new DiverterException($"Invalid type argument {typeof(T).Name}. Only interface or class types are supported");
        }
    }
}