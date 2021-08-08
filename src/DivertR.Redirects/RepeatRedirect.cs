using System;
using System.Threading;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class RepeatRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IRedirect<TTarget> _innerRedirect;
        private readonly int _repeatCount;
        private long _callCount;

        public RepeatRedirect(IVia<TTarget> via, IRedirect<TTarget> innerRedirect, int repeatCount)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(repeatCount));
            }
            
            _via = via;
            _innerRedirect = innerRedirect;
            _repeatCount = repeatCount;
        }

        public ICallConstraint<TTarget> CallConstraint => _innerRedirect.CallConstraint;
        
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow
            {
                count = Interlocked.Exchange(ref _callCount, (long) _repeatCount + 1);
            }

            return count <= _repeatCount 
                ? _innerRedirect.Call(callInfo)
                : _via.Relay.CallNext(callInfo);
        }
    }
}
