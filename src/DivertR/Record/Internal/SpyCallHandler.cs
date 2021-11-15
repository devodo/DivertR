using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Record.Internal
{
    internal class SpyCallHandler<TTarget, TMap> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ConcurrentQueue<TMap> _mappedCalls = new ConcurrentQueue<TMap>();
        
        private readonly IRelay<TTarget> _relay;
        private readonly Func<IRecordedCall<TTarget>, TMap> _mapper;
        
        public IReadOnlyCollection<TMap> MappedCalls => _mappedCalls;

        public SpyCallHandler(IRelay<TTarget> relay, Func<IRecordedCall<TTarget>, TMap> mapper)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _mapper = mapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var recordedCall = new RecordedCall<TTarget>(callInfo);
            object? returnValue;

            try
            {
                returnValue = _relay.CallNext();
                recordedCall.SetReturned(returnValue);
            }
            catch (Exception ex)
            {
                recordedCall.SetException(ex);
                throw;
            }

            var mapped = _mapper.Invoke(recordedCall);
            _mappedCalls.Enqueue(mapped);
            
            return returnValue;
        }
    }
}