using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RouterInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<RedirectRoute<T>?> _getRedirectRoute;

        public RouterInterceptor(T? original, Func<RedirectRoute<T>?> getRedirectRoute)
        {
            _original = original;
            _getRedirectRoute = getRedirectRoute;
        }

        public void Intercept(IInvocation invocation)
        {
            var route = _getRedirectRoute();
            var redirect = route?.CallRelay.BeginCall(_original, route.Redirects, invocation);

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
                
                // ReSharper disable once SuspiciousTypeConversion.Global
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                invocation.Proceed();
            }
            finally
            {
                route!.CallRelay.EndCall(invocation);
            }
        }
    }
}