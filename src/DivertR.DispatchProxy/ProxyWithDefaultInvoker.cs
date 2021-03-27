using System;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.DispatchProxy
{
    internal class ProxyWithDefaultInvoker<T> : IDispatchProxyInvoker where T : class
    {
        private readonly T _proxy;
        private readonly T? _original;
        private readonly Func<IProxyCall<T>?> _getProxyCall;
        
        public ProxyWithDefaultInvoker(T proxy, T? original, Func<IProxyCall<T>?> getProxyCall)
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
            
            var callInfo = new CallInfo<T>(_proxy, _original, targetMethod, args);

            return proxyCall.Call(callInfo)!;
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