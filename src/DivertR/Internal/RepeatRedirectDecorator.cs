using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class RepeatRedirectDecorator : IRedirect
    {
        private readonly IRedirect _innerRedirect;
        private readonly int _repeatCount;
        private long _callCount;

        public RepeatRedirectDecorator(IRedirect innerRedirect, int repeatCount)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(repeatCount));
            }
            
            _innerRedirect = innerRedirect;
            _repeatCount = repeatCount;
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

            if (count == long.MinValue) // overflow
            {
                count = Interlocked.Exchange(ref _callCount, (long) _repeatCount + 1);
            }

            return count <= _repeatCount 
                ? _innerRedirect.Handle(call)
                : call.Relay.CallNext();
        }
    }
}
