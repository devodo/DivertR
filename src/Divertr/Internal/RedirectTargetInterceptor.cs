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
            var redirectContext = _callRelay.Peek();
            var redirect = redirectContext.BeginNextRedirect(invocation);

            if (redirect == null)
            {
                if (redirectContext.Original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }

                ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectContext.Original);
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
                ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirect.Target);
                invocation.Proceed();
            }
            finally
            {
                redirectContext.EndRedirect();
            }
        }
    }
}