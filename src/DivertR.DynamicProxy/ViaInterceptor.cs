using System;
using Castle.DynamicProxy;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DynamicProxy
{
    internal class ViaInterceptor<T> : IInterceptor where T : class
    {
        private readonly T? _original;
        private readonly Func<IViaState<T>?> _getViaState;

        public ViaInterceptor(T? original, Func<IViaState<T>?> getViaState)
        {
            _original = original;
            _getViaState = getViaState;
        }

        public void Intercept(IInvocation invocation)
        {
            var viaState = _getViaState();

            if (viaState == null)
            {
                DefaultProceed(invocation);
                return;
            }
            
            var call = new DynamicProxyCall(invocation);
            var redirect = viaState.RelayState.BeginCall(_original, viaState.Redirects, call);

            if (redirect == null)
            {
                DefaultProceed(invocation);
                return;
            }

            try
            {
                invocation.ReturnValue = redirect.Invoke(invocation.Method, invocation.Arguments)!;

                // ReSharper disable once SuspiciousTypeConversion.Global
                //((IChangeProxyTarget) invocation).ChangeInvocationTarget(redirect.Target);
                //invocation.Proceed();
            }
            finally
            {
                viaState.RelayState.EndCall(call);
            }
        }

        private void DefaultProceed(IInvocation invocation)
        {
            if (_original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }

            invocation.Proceed();
        }
    }
}