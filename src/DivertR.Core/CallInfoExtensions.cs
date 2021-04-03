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
        
        public static object? Invoke<TTarget>(this CallInfo<TTarget> callInfo, TTarget target) where TTarget : class
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            var delegateInternal = callInfo.Method.ToDelegate(typeof(TTarget));
            return delegateInternal.Invoke(target, callInfo.Arguments.InternalArgs);
        }
    }
}