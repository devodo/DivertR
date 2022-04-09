using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal static class CallInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Invoke(this CallInfo callInfo, object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            var delegateInternal = callInfo.Method.ToDelegate(target.GetType());
            return delegateInternal.Invoke(target, callInfo.Arguments.InternalArgs);
        }
    }
}