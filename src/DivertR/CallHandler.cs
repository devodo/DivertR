using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallHandler<TTarget> : ICallHandler where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(IRedirectCall call)
        {
            if (!(call is IRedirectCall<TTarget> callOf))
            {
                throw new ArgumentException($"Redirect call target type mismatch: {call.GetType()}", nameof(call));
            }

            return Call(callOf);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract object? Call(IRedirectCall<TTarget> call);
    }
}