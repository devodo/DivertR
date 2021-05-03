using System;
using System.Threading;
using DivertR.Core;

namespace DivertR.Redirects
{
    public class RecursRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly IVia<TTarget> _via;
        private readonly IRedirect<TTarget> _innerRedirect;
        private readonly int _recurCount;
        private long _callCount;

        public RecursRedirect(IVia<TTarget> via, IRedirect<TTarget> innerRedirect, int recurCount)
        {
            if (recurCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(recurCount));
            }
            
            _via = via;
            _innerRedirect = innerRedirect;
            _recurCount = recurCount;
        }

        public ICallConstraint<TTarget> CallConstraint => _innerRedirect.CallConstraint;
        
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow
            {
                count = Interlocked.Exchange(ref _callCount, (long) _recurCount + 1);
            }

            return count <= _recurCount 
                ? _innerRedirect.Call(callInfo)
                : _via.Relay.CallNext(callInfo);
        }
    }
}
