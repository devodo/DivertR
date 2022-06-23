using System;

namespace DivertR.Record.Internal
{
    internal class RecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
        private readonly object _returnedLock = new object();
        
        private ICallReturn? _callReturn;
        
        public RecordedCall(ICallInfo<TTarget> callInfo)
        {
            CallInfo = callInfo;
        }
        
        public ICallInfo<TTarget> CallInfo { get; }

        public CallArguments Args => CallInfo.Arguments;
        
        public ICallReturn? Returned
        {
            get
            {
                lock (_returnedLock)
                {
                    return _callReturn;
                }
            }

            private set
            {
                lock (_returnedLock)
                {
                    _callReturn = value;
                }
            }
        }
        
        public void SetReturned(object? returnedObject)
        {
            Returned = new CallReturn(returnedObject, null);
        }

        public void SetException(Exception exception)
        {
            Returned = new CallReturn(null, exception);
        }
    }

    internal class RecordedCallInternal<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
        private readonly IRecordedCall<TTarget> _recordedCall;

        protected RecordedCallInternal(IRecordedCall<TTarget> recordedCall)
        {
            _recordedCall = recordedCall;
        }

        public ICallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
        public ICallReturn? Returned => _recordedCall.Returned;
        public CallArguments Args => _recordedCall.Args;
    }
    
    internal class RecordedCall<TTarget, TArgs> : RecordedCallInternal<TTarget>, IRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
        public RecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall)
        {
            Args = args;
        }
        
        public new TArgs Args { get; }
        CallArguments IRecordedCall<TTarget>.Args => CallInfo.Arguments;
    }
}
