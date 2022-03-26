using System;
using System.Threading;

namespace DivertR.Internal
{
    internal class RepeatCallHandler : ICallHandler
    {
        private readonly IVia _via;
        private readonly ICallHandler _innerCallHandler;
        private readonly int _repeatCount;
        private long _callCount;

        public RepeatCallHandler(IVia via, ICallHandler innerCallHandler, int repeatCount)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(repeatCount));
            }
            
            _via = via;
            _innerCallHandler = innerCallHandler;
            _repeatCount = repeatCount;
        }

        public object? Call(CallInfo callInfo)
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
