using System;
using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class ViaInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<IViaState<T>?> _getRedirectRoute;

        public ViaInterceptor(T? original, Func<IViaState<T>?> getRedirectRoute)
        {
            _original = original;
            _getRedirectRoute = getRedirectRoute;
        }

        public void Intercept(IInvocation invocation)
        {
            var call = new DynamicProxyCall(invocation);
            var route = _getRedirectRoute();
            var redirect = route?.RelayState.BeginCall(_original, route.Redirects, call);

            if (redirect == null)
            {
                if (_original == null)
                {
                    throw new DiverterException("The original instance reference is null");
                }

                invocation.Proceed();
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
                route!.RelayState.EndCall(call);
            }
        }
    }
}