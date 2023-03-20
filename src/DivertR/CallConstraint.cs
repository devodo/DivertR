using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallConstraint<TTarget> : ICallConstraint where TTarget : class?
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool IsMatch(ICallInfo<TTarget> callInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            if (callInfo is not ICallInfo<TTarget> callOfTTarget)
            {
                throw new ArgumentException($"Via target type {typeof(TTarget)} invalid for ICallInfo type: {callInfo.GetType()}", nameof(callInfo));
            }
            
            return IsMatch(callOfTTarget);
        }
    }
}