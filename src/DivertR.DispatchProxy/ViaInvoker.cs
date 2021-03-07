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
            var viaState = _getViaState();

            if (viaState == null)
            {
                return DefaultProceed(targetMethod, args);
            }
            
            var call = new DispatchProxyCall(targetMethod, args);
            var redirect = viaState.RelayState.BeginCall(_original, viaState.Redirects, call);

            if (redirect == null)
            {
                return DefaultProceed(targetMethod, args);
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
        
        private object DefaultProceed(MethodInfo targetMethod, object[] args)
        {
            if (_original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }

            return targetMethod.ToDelegate(typeof(T)).Invoke(_original, args);
        }
    }
}