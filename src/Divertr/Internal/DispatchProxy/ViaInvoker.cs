using System;
using System.Reflection;

namespace DivertR.Internal.DispatchProxy
{
    internal class ViaInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly T? _original;
        private readonly Func<ViaWay<T>?> _getRedirectRoute;

        public ViaInvoker(T? original, Func<ViaWay<T>?> getRedirectRoute)
        {
            _original = original;
            _getRedirectRoute = getRedirectRoute;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var call = new DispatchProxyCall(targetMethod, args);
            var route = _getRedirectRoute();
            var redirect = route?.Relay.BeginCall(_original, route.Redirects, call);

            if (redirect == null)
            {
                if (_original == null)
                {
                    throw new DiverterException("The original instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(_original, args);
            }

            try
            {
                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(redirect.Target, args);
            }
            finally
            {
                route!.Relay.EndCall(call);
            }
        }
    }
}