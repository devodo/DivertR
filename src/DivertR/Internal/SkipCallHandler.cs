﻿using System;
using System.Threading;

namespace DivertR.Internal
{
    internal class SkipCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ICallHandler<TTarget> _innerCallHandler;
        private readonly int _skipCount;
        private long _callCount;

        public SkipCallHandler(ICallHandler<TTarget> innerCallHandler, int skipCount)
        {
            if (skipCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(skipCount));
            }
            
            _innerCallHandler = innerCallHandler;
            _skipCount = skipCount;
        }

        public object? Handle(IRedirectCall<TTarget> call)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow case
            {
                count = Interlocked.Exchange(ref _callCount, (long) _skipCount + 1);
            }

            return count > _skipCount 
                ? _innerCallHandler.Handle(call)
                : call.Relay.CallNext();
        }
    }
}
