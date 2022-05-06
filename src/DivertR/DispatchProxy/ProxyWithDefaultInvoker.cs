using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Internal;

namespace DivertR.DispatchProxy
{
    public class ProxyWithDefaultInvoker<TTarget> : IProxyInvoker where TTarget : class
    {
        private readonly TTarget _proxy;
        private readonly TTarget? _root;
        private readonly Func<IProxyCall<TTarget>?> _getProxyCall;
        
        public ProxyWithDefaultInvoker(TTarget proxy, TTarget? root, Func<IProxyCall<TTarget>?> getProxyCall)
        {
            _proxy = proxy;
            _root = root;
            _getProxyCall = getProxyCall;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Invoke(MethodInfo targetMethod, object[] args)
        {
            var proxyCall = _getProxyCall.Invoke();

            if (proxyCall == null)
            {
                return DefaultProceed(targetMethod, args);
            }
            
            var callInfo = CallInfoFactory.Create(_proxy, _root, targetMethod, args);

            return proxyCall.Call(callInfo)!;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? DefaultProceed(MethodInfo targetMethod, object[] args)
        {
            if (_root == null)
            {
                throw new DiverterNullRootException("Root instance is null");
            }

            return targetMethod.ToDelegate<TTarget>().Invoke(_root, args);
        }
    }
}