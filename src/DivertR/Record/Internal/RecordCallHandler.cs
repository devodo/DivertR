using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal class RecordCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;
        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public ICallStream<TTarget> CallStream { get; }

        public RecordCallHandler(IRelay<TTarget> relay)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            CallStream = new CallStream<TTarget>(_recordedCalls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var recordedCall = new RecordedCall<TTarget>(callInfo);
            _recordedCalls.Enqueue(recordedCall);
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
            
            return returnValue;
        }
    }
}