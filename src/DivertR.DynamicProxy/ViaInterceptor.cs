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
            
            var call = new CallInfo<T>((T) invocation.Proxy, _original, invocation.Method, invocation.Arguments);
            invocation.ReturnValue = viaState.RelayState.CallBegin(viaState.Redirects, call);
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