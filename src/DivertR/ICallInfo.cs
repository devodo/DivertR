using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface ICallInfo
    {
        public Type TargetType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
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
    }
}