using System;
using System.Threading;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class RepeatCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly ICallHandler<TTarget> _innerCallHandler;
        private readonly int _repeatCount;
        private long _callCount;

        public RepeatCallHandler(IVia<TTarget> via, ICallHandler<TTarget> innerCallHandler, int repeatCount)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(repeatCount));
            }
            
            _via = via;
            _innerCallHandler = innerCallHandler;
            _repeatCount = repeatCount;
        }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow
            {
                count = Interlocked.Exchange(ref _callCount, (long) _repeatCount + 1);
            }

            return count <= _repeatCount 
                ? _innerCallHandler.Call(callInfo)
                : _via.Relay.CallNext();
        }
    }
}
