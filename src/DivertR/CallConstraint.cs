﻿using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class CallConstraint<TTarget> : ICallConstraint where TTarget : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            if (!(callInfo is CallInfo<TTarget> typedCallInfo))
            {
                throw new ArgumentException($"Call target type mismatch: {callInfo.GetType()}", nameof(callInfo));
            }
            
            return IsMatch(typedCallInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool IsMatch(CallInfo<TTarget> callInfo);
    }
}
