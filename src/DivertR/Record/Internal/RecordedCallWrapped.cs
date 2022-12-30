using System;

namespace DivertR.Record.Internal
{
    internal class RecordedCallWrapped : IRecordedCall
    {
        private readonly IRecordedCall _recordedCall;

        protected RecordedCallWrapped(IRecordedCall recordedCall)
        {
            _recordedCall = recordedCall;
        }
        
        public ICallInfo CallInfo => _recordedCall.CallInfo;
        public CallArguments Args => _recordedCall.Args;
        public object? Return => _recordedCall.Return;
        public object? ReturnOrDefault => _recordedCall.ReturnOrDefault;
        public Exception? Exception => _recordedCall.Exception;
        public Exception? RawException => _recordedCall.RawException;
        public bool IsCompleted => _recordedCall.IsCompleted;
        public bool IsReturned => _recordedCall.IsReturned;
    }
    
    internal class RecordedCallWrapped<TTarget> : RecordedCallWrapped, IRecordedCall<TTarget> where TTarget : class?
    {
        private readonly IRecordedCall<TTarget> _recordedCall;

        protected RecordedCallWrapped(IRecordedCall<TTarget> recordedCall) : base(recordedCall)
        {
            _recordedCall = recordedCall;
        }

        public new ICallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
    }
}