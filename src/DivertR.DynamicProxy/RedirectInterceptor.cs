using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class RedirectInterceptor<T> : IInterceptor where T : class
    {
        private readonly IRelayState<T> _relayState;

        public RedirectInterceptor(IRelayState<T> relayState)
        {
            _relayState = relayState;
        }

        public void Intercept(IInvocation invocation)
        {
            var call = new DynamicProxyCall(invocation);
            var redirect = _relayState.BeginNextRedirect(call);
            
            if (redirect == null)
            {
                var original = _relayState.Original;
                if (original == null)
                {
                    throw new DiverterException("Proxy original instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(original, invocation.Arguments);
                
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(original);
                //invocation.Proceed();

                return;
            }

            try
            {
                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                invocation.ReturnValue =
                    invocation.Method.ToDelegate(typeof(T)).Invoke(redirect.Target, invocation.Arguments);

                // ReSharper disable once SuspiciousTypeConversion.Global
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                //invocation.Proceed();
            }
            finally
            {
                _relayState.EndRedirect(call);
            }
        }
    }
}