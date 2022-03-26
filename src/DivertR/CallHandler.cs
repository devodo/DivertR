using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallHandler<TTarget> : ICallHandler where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            if (!(callInfo is CallInfo<TTarget> typedCallInfo))
            {
                throw new ArgumentException($"Call target type mismatch: {callInfo.GetType()}", nameof(callInfo));
            }
            
            return Call(typedCallInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract object? Call(CallInfo<TTarget> callInfo);
    }
}