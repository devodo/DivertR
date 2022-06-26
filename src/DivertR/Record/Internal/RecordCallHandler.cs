using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DivertR.Record.Internal
{
    internal class RecordCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ConcurrentQueue<RecordedCall<TTarget>> _recordedCalls = new ConcurrentQueue<RecordedCall<TTarget>>();

        public IRecordStream<TTarget> RecordStream { get; }

        public RecordCallHandler()
        {
            RecordStream = new RecordStream<TTarget>(_recordedCalls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var recordedCall = new RecordedCall<TTarget>(call.CallInfo);
            _recordedCalls.Enqueue(recordedCall);
            object? returnValue;

            try
            {
                returnValue = call.Relay.CallNext();
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
    
    internal class RecordCallHandler : ICallHandler
    {
        private readonly ConcurrentQueue<RecordedCall> _recordedCalls = new ConcurrentQueue<RecordedCall>();

        public IRecordStream RecordStream { get; }

        public RecordCallHandler()
        {
            RecordStream = new RecordStream(_recordedCalls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var recordedCall = new RecordedCall(call.CallInfo);
            _recordedCalls.Enqueue(recordedCall);
            object? returnValue;

            try
            {
                returnValue = call.Relay.CallNext();
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