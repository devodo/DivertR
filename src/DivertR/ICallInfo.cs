using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallInfo
    {
        object Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        object? Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
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

        public ICallInfo Create(CallArguments args);
        public ICallInfo Create(MethodInfo method, CallArguments args);
    }
    
    public interface ICallInfo<out TTarget> : ICallInfo where TTarget : class?
    {
        [NotNull]
        new TTarget Proxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        new TTarget? Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        new ICallInfo<TTarget> Create(MethodInfo method, CallArguments args);
        new ICallInfo<TTarget> Create(CallArguments args);
    }
}