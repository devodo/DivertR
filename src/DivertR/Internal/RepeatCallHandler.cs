using System;
using System.Threading;

namespace DivertR.Internal
{
    internal class RepeatCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly ICallHandler<TTarget> _innerCallHandler;
        private readonly int _repeatCount;
        private long _callCount;

        public RepeatCallHandler(ICallHandler<TTarget> innerCallHandler, int repeatCount)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(repeatCount));
            }
            
            _innerCallHandler = innerCallHandler;
            _repeatCount = repeatCount;
        }

        public object? Handle(IRedirectCall<TTarget> call)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow
            {
                count = Interlocked.Exchange(ref _callCount, (long) _repeatCount + 1);
            }

            return count <= _repeatCount 
                ? _innerCallHandler.Handle(call)
                : call.Relay.CallNext();
        }
    }
}
