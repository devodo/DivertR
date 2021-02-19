using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class OriginalTargetInterceptor<T> : IInterceptor where T : class
    {
        private readonly CallRelay<T> _callRelay;

        public OriginalTargetInterceptor(CallRelay<T> callRelay)
        {
            _callRelay = callRelay;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirectionContext = _callRelay.Peek();
            
            if (redirectionContext == null)
            {
                throw new DiverterException("Members of this instance may only be accessed from within the context of its DivertR proxy calls");
            }

            if (redirectionContext.Original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectionContext.Original);
            invocation.Proceed();
        }
    }
}