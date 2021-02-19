using System;
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
            
            if (redirectContext == null)
            {
                throw new DiverterException("Members of this instance may only be accessed from within the context of its DivertR proxy calls");
            }

            if (redirectContext.BeginNextRedirect(invocation, out var redirect))
            {
                try
                {
                    if (redirect == null)
                    {
                        throw new DiverterException("The redirect instance reference is null");
                    }
                    
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirect);
                    invocation.Proceed();
                }
                finally
                {
                    redirectContext.EndRedirect();
                }

                return;
            }
            
            if (redirectContext.Original == null)
            {
                throw new DiverterException("Proxy original instance reference is null");
            }

            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectContext.Original);
            invocation.Proceed();
        }
    }
}