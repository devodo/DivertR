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
            var redirectRelay = _callRelay.Current;
            
            if (redirectRelay.Original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((IChangeProxyTarget)invocation).ChangeInvocationTarget(redirectRelay.Original);
            invocation.Proceed();
        }
    }
}