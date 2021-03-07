using System;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class ViaInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly T? _original;
        private readonly Func<IViaState<T>?> _getViaState;

        public ViaInvoker(T? original, Func<IViaState<T>?> getViaState)
        {
            _original = original;
            _getViaState = getViaState;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var call = new DispatchProxyCall(targetMethod, args);
            var viaState = _getViaState();
            var redirect = viaState?.RelayState.BeginCall(_original, viaState.Redirects, call);

            if (redirect == null)
            {
                if (_original == null)
                {
                    throw new DiverterException("The original instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(_original, args);
            }

            try
            {
                if (redirect.Target == null)
                {
                    throw new DiverterException("The redirect instance reference is null");
                }
                
                return targetMethod.ToDelegate(typeof(T)).Invoke(redirect.Target, args);
            }
            finally
            {
                viaState!.RelayState.EndCall(call);
            }
        }
    }
}