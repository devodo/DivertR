using System;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class ViaInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly T _proxy;
        private readonly T? _original;
        private readonly Func<IViaState<T>?> _getViaState;

        public ViaInvoker(T proxy, T? original, Func<IViaState<T>?> getViaState)
        {
            _proxy = proxy;
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
            
            var call = new CallInfo(targetMethod, args);
            return viaState.RelayState.CallBegin(_proxy, _original, viaState.Redirects, call)!;
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