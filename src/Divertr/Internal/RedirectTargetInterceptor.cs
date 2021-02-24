using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RedirectTargetInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallRelay<T> _callRelay;

        public RedirectTargetInterceptor(CallRelay<T> callRelay)
        {
            _callRelay = callRelay;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectRelay = _callRelay.Current;
            var redirect = redirectRelay.BeginNextRedirect(invocation);
            
            if (redirect == null)
            {
                if (redirectRelay.Original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }

                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirectRelay.Original);
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
                redirectRelay.EndRedirect();
            }
        }
    }
}