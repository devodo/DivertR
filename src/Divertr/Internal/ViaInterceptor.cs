using System;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class ViaInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<ViaWay<T>?> _getRedirectRoute;

        public ViaInterceptor(T? original, Func<ViaWay<T>?> getRedirectRoute)
        {
            _original = original;
            _getRedirectRoute = getRedirectRoute;
        }

        public void Intercept(IInvocation invocation)
        {
            var route = _getRedirectRoute();
            var redirect = route?.Relay.BeginCall(_original, route.Redirects, invocation);

            if (redirect == null)
            {
                if (_original == null)
                {
                    throw new DiverterException("The original instance reference is null");
                }

                invocation.Proceed();
                return;
            }

            try
            {
                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }

                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(redirect.Target, invocation.Arguments);

                // ReSharper disable once SuspiciousTypeConversion.Global
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                //invocation.Proceed();
            }
            finally
            {
                route!.Relay.EndCall(invocation);
            }
        }
    }
}