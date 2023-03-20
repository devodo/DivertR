using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallHandler<TTarget> : ICallHandler where TTarget : class?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract object? Handle(IRedirectCall<TTarget> call);
        
        public object? Handle(IRedirectCall call)
        {
            if (call is not IRedirectCall<TTarget> callOfTTarget)
            {
                throw new ArgumentException($"Via target type {typeof(TTarget)} invalid for IRedirectCall type: {call.GetType()}", nameof(call));
            }

            return Handle(callOfTTarget);
        }
    }
}