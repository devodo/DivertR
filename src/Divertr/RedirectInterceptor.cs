using System;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;

        public RedirectInterceptor(CallContext<T> callContext)
        {
            _callContext = callContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectionContext = _callContext.Peek();
            
            if (redirectionContext == null)
            {
                throw new InvalidOperationException("Replaced instances may only be accessed within the context of a Diverter Proxy call");
            }

            if (redirectionContext.MoveNext(invocation, out var redirect))
            {
                try
                {
                    ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirect);

                    if (redirect == null)
                    {
                        throw new DiverterException("Redirect reference not set to an instance of an object.");
                    }

                    invocation.Proceed();
                }
                finally
                {
                    redirectionContext.MoveBack();
                }

                return;
            }

            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectionContext.Origin);
            invocation.Proceed();
        }
    }
}