using System;
using System.Reflection;
using DivertR.Core;

namespace DivertR.DispatchProxy
{
    internal class ProxyWithDefaultInvoker<TTarget> : IProxyInvoker where TTarget : class
    {
        private readonly TTarget _proxy;
        private readonly TTarget? _original;
        private readonly Func<IProxyCall<TTarget>?> _getProxyCall;
        
        public ProxyWithDefaultInvoker(TTarget proxy, TTarget? original, Func<IProxyCall<TTarget>?> getProxyCall)
        {
            _proxy = proxy;
            _original = original;
            _getProxyCall = getProxyCall;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            var proxyCall = _getProxyCall.Invoke();

            if (proxyCall == null)
            {
                return DefaultProceed(targetMethod, args);
            }
            
            var callInfo = new CallInfo<TTarget>(_proxy, _original, targetMethod, args);

            return proxyCall.Call(callInfo)!;
        }
        
        private object DefaultProceed(MethodInfo targetMethod, object[] args)
        {
            if (_original == null)
            {
                throw new DiverterException("The original instance reference is null");
            }

            return targetMethod.ToDelegate<TTarget>().Invoke(_original, args);
        }
    }
}