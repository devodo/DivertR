using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DivertR.Internal
{
    internal class SkipViaDecorator : IVia
    {
        private readonly IVia _innerVia;
        private readonly int _skipCount;
        private long _callCount;

        public SkipViaDecorator(IVia innerVia, int skipCount)
        {
            if (skipCount < 0)
            {
                throw new ArgumentException("Must be greater than or equal to zero", nameof(skipCount));
            }
            
            _innerVia = innerVia;
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
                ? _innerVia.Handle(call)
                : call.Relay.CallNext();
        }
    }
}
