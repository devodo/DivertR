using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class ReplacedInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;

        public ReplacedInterceptor(CallContext<T> callContext)
        {
            _callContext = callContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectContext = _callContext.Peek();
            
            if (redirectContext == null)
            {
                throw new InvalidOperationException("Replaced instances may only be accessed within the context of a Diverter Proxy call");
            }

            if (redirectContext.MoveNext(invocation, out var redirect))
            {
                try
                {
                    if (redirect == null)
                    {
                        throw new DiverterException("Redirect target reference not set to an instance of an object.");
                    }
                    
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirect);
                    invocation.Proceed();
                }
                finally
                {
                    redirectContext.MoveBack();
                }

                return;
            }
            
            if (redirectContext.Origin == null)
            {
                throw new DiverterException("Original reference not set to an instance of an object.");
            }

            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectContext.Origin);
            invocation.Proceed();
        }
    }
}