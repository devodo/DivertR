using Castle.DynamicProxy;

namespace DivertR.Internal
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
            var redirectRelay = _callRelay.BeginNextRedirect(invocation);
            
            if (redirectRelay == null)
            {
                var original = _callRelay.Current.Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(original, invocation.Arguments);
                
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirectRelay.Original);
                //invocation.Proceed();

                return;
            }

            try
            {
                var redirect = redirectRelay.Current.Target;
                if (redirect == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(redirect, invocation.Arguments);

                // ReSharper disable once SuspiciousTypeConversion.Global
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                //invocation.Proceed();
            }
            finally
            {
                _callRelay.EndRedirect(invocation);
            }
        }
    }
}