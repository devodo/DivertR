﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Internal;

namespace DivertR.DispatchProxy
{
    public class ProxyWithDefaultInvoker<TTarget> : IProxyInvoker where TTarget : class
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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