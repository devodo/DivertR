using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class DiverterInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<RedirectRoute<T>?> _getRedirectRoute;

        public DiverterInterceptor(T? original, Func<RedirectRoute<T>?> getRedirectRoute)
        {
            _original = original;
            _getRedirectRoute = getRedirectRoute;
        }

        public void Intercept(IInvocation invocation)
        {
            var route = _getRedirectRoute();

            if (route == null ||
                !route.TryBeginCall(_original, invocation, out T? redirect))
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
                // ReSharper disable once SuspiciousTypeConversion.Global
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect);
                invocation.Proceed();
            }
            finally
            {
                route.EndCall(invocation);
            }
        }
    }
}