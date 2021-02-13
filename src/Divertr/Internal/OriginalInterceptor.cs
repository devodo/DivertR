using System;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class OriginalInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;

        public OriginalInterceptor(CallContext<T> callContext)
        {
            _callContext = callContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectionContext = _callContext.Peek();
            
            if (redirectionContext == null)
            {
                throw new DiverterException("Call context instances may only be accessed within a Diverter Proxy call");
            }

            if (redirectionContext.Origin == null)
            {
                throw new DiverterException("Original reference not set to an instance of an object.");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectionContext.Origin);
            invocation.Proceed();
        }
    }
}