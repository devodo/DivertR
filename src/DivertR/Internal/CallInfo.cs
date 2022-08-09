using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CallInfo<TTarget> : ICallInfo<TTarget> where TTarget : class?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallInfo([DisallowNull] TTarget proxy, TTarget? root, MethodInfo method, CallArguments arguments)
        {
            Proxy = proxy;
            Root = root;
            Method = method;
            Arguments = arguments;
        }
        
        [NotNull]
        public TTarget Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public TTarget? Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        object? ICallInfo.Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Root;
        }
        
        object ICallInfo.Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Proxy;
        }

        public MethodInfo Method
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public CallArguments Arguments
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public ICallInfo<TTarget> Create(MethodInfo method, CallArguments args)
        {
            return new CallInfo<TTarget>(Proxy, Root, method, args);
        }

        public ICallInfo<TTarget> Create(CallArguments args)
        {
            return new CallInfo<TTarget>(Proxy, Root, Method, args);
        }

        ICallInfo ICallInfo.Create(MethodInfo method, CallArguments args)
        {
            return Create(method, args);
        }

        ICallInfo ICallInfo.Create(CallArguments args)
        {
            return Create(args);
        }
    }
}