using System;

namespace DivertR.Core
{
    public static class CallInfoExtensions
    {
        public static Func<object, CallArguments, object> ToDelegate<T>(this CallInfo<T> callInfo) where T : class
        {
            var delegateInternal = callInfo.Method.ToDelegate<T>();

            return (target, arguments) => delegateInternal.Invoke(target, arguments.InternalArgs);
        }
        
        public static object? Invoke<T>(this CallInfo<T> callInfo, T target) where T : class
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            var delegateInternal = callInfo.Method.ToDelegate(typeof(T));
            return delegateInternal.Invoke(target, callInfo.CallArguments.InternalArgs);
        }
    }
}