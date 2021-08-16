using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class Relay<TTarget> : IRelay<TTarget> where TTarget : class
    {
        private readonly RelayContext<TTarget> _relayContext;
        private readonly Lazy<TTarget> _next;
        private readonly Lazy<TTarget> _original;

        public TTarget Next => _next.Value;
        public TTarget Original => _original.Value;

        public Redirect<TTarget> Redirect => _relayContext.Redirect;

        public CallInfo<TTarget> CallInfo => _relayContext.CallInfo;

        public Relay(RelayContext<TTarget> relayContext, IProxyFactory proxyFactory)
        {
            _relayContext = relayContext;
            _next = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new NextProxyCall<TTarget>(_relayContext)));
            _original = new Lazy<TTarget>(() => proxyFactory.CreateProxy(new OriginalProxyCall<TTarget>(_relayContext)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext()
        {
            return _relayContext.CallNext();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallNext(MethodInfo method, CallArguments callArguments)
        {
            return _relayContext.CallNext(method, callArguments);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal()
        {
            return _relayContext.CallOriginal();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? CallOriginal(MethodInfo method, CallArguments callArguments)
        {
            return _relayContext.CallOriginal(method, callArguments);
        }
    }
}