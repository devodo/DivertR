using System;
using System.Threading;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class SkipRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IRedirect<TTarget> _innerRedirect;
        private readonly int _skipCount;
        private long _callCount;

        public SkipRedirect(IVia<TTarget> via, IRedirect<TTarget> innerRedirect, int skipCount)
        {
            if (skipCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(skipCount));
            }
            
            _via = via;
            _innerRedirect = innerRedirect;
            _skipCount = skipCount;
        }

        public ICallConstraint<TTarget> CallConstraint => _innerRedirect.CallConstraint;
        
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow case
            {
                count = Interlocked.Exchange(ref _callCount, (long) _skipCount + 1);
            }

            return count > _skipCount 
                ? _innerRedirect.Call(callInfo)
                : _via.Relay.CallNext(callInfo);
        }
    }
}
