using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal static class CallInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Invoke<TTarget>(this ICallInfo<TTarget> callInfo, object target) where TTarget : class
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            var delegateInternal = callInfo.Method.ToDelegate(typeof(TTarget));
            return delegateInternal.Invoke(target, callInfo.Arguments.InternalArgs);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Invoke(this ICallInfo callInfo, object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            var delegateInternal = callInfo.Method.ToDelegate(target.GetType());
            return delegateInternal.Invoke(target, callInfo.Arguments.InternalArgs);
        }
    }
}