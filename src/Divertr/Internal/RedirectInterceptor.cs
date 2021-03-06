using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly Relay<T> _relay;

        public RedirectInterceptor(Relay<T> relay)
        {
            _relay = relay;
        }

        public void Intercept(IInvocation invocation)
        {
            var redirect = _relay.BeginNextRedirect(invocation);
            
            if (redirect == null)
            {
                var original = _relay.Current.Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                //invocation.ReturnValue =
                //    invocation.Method.ToDelegate(typeof(T)).Invoke(original, invocation.Arguments);
                
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(original);
                invocation.Proceed();

                return;
            }

            try
            {

                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                //invocation.ReturnValue =
                //    invocation.Method.ToDelegate(typeof(T)).Invoke(redirect.Target, invocation.Arguments);

                // ReSharper disable once SuspiciousTypeConversion.Global
                ((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                invocation.Proceed();
            }
            finally
            {
                _relay.EndRedirect(invocation);
            }
        }
    }
}