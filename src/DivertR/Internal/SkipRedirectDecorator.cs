using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class SkipRedirectDecorator : IRedirect
    {
        private readonly IRedirect _innerRedirect;
        private readonly int _skipCount;
        private long _callCount;

        public SkipRedirectDecorator(IRedirect innerRedirect, int skipCount)
        {
            if (skipCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(skipCount));
            }
            
            _innerRedirect = innerRedirect;
            _skipCount = skipCount;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var count = Interlocked.Increment(ref _callCount);

            if (count == long.MinValue) // overflow case
            {
                count = Interlocked.Exchange(ref _callCount, (long) _skipCount + 1);
            }

            return count > _skipCount 
                ? _innerRedirect.Handle(call)
                : call.Relay.CallNext();
        }
    }
}
