using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RootTargetInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallContext<T> _callContext;

        public RootTargetInterceptor(CallContext<T> callContext)
        {
            _callContext = callContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectionContext = _callContext.Peek();
            
            if (redirectionContext == null)
            {
                throw new DiverterException("Root target may only be accessed within the context of a Diversion call");
            }

            if (redirectionContext.Root == null)
            {
                throw new DiverterException("Root target not set to an instance of an object.");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectionContext.Root);
            invocation.Proceed();
        }
    }
}