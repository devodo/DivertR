using System;

namespace DivertR.Record.Internal
{
    internal class RecordedCall : IRecordedCall
    {
        private readonly object _returnedLock = new object();
        
        private ICallReturn? _callReturn;
        
        public RecordedCall(ICallInfo callInfo)
        {
            CallInfo = callInfo;
        }
        
        public ICallInfo CallInfo { get; }

        ICallInfo IRecordedCall.CallInfo => CallInfo;

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
    
    internal class RecordedCall<TTarget> : RecordedCall, IRecordedCall<TTarget> where TTarget : class
    {
        public RecordedCall(ICallInfo<TTarget> callInfo) : base(callInfo)
        {
            CallInfo = callInfo;
        }
        
        public new ICallInfo<TTarget> CallInfo { get; }
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
        ICallInfo IRecordedCall.CallInfo => CallInfo;

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
        CallArguments IRecordedCall.Args => CallInfo.Arguments;
    }
}
