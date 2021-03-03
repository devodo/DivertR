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
                if (_callRelay.Original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(_callRelay.Original, invocation.Arguments);
                
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirectRelay.Original);
                //invocation.Proceed();

                return;
            }

            try
            {
                if (redirectRelay.Current.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(redirectRelay.Current.Target, invocation.Arguments);

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