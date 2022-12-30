using System;

namespace DivertR.Record.Internal
{
    internal class RecordedCall : IRecordedCall
    {
        private readonly object _returnedLock = new();

        private CallReturn _callReturn = CallReturn.None;

        public RecordedCall(ICallInfo callInfo)
        {
            CallInfo = callInfo;
        }
        
        public ICallInfo CallInfo { get; }

        public CallArguments Args => CallInfo.Arguments;

        public object? Return
        {
            get
            {
                var callReturn = CallReturn;

                if (!callReturn.IsReturned)
                {
                    throw new DiverterException($"The call does not have a return value. It probably did not return due to an exception being thrown.");
                }
                
                return CallReturn.Value;
            }
        }

        public object? ReturnOrDefault => CallReturn.Value;
        
        public Exception? Exception => CallReturn.Exception;
        
        public Exception? RawException => CallReturn.RawException;

        public bool IsCompleted => CallReturn.IsCompleted;
        
        public bool IsReturned => CallReturn.IsReturned;

        public void SetReturned(object? returnedObject)
        {
            CallReturn = new CallReturn(returnedObject);
        }

        public void SetException(Exception exception)
        {
            CallReturn = new CallReturn(exception);
        }
        
        private CallReturn CallReturn
        {
            get
            {
                lock (_returnedLock)
                {
                    return _callReturn;
                }
            }

            set
            {
                lock (_returnedLock)
                {
                    _callReturn = value;
                }
            }
        }
    }
    
    internal class RecordedCall<TTarget> : RecordedCall, IRecordedCall<TTarget> where TTarget : class?
    {
        public RecordedCall(ICallInfo<TTarget> callInfo) : base(callInfo)
        {
            CallInfo = callInfo;
        }
        
        public new ICallInfo<TTarget> CallInfo { get; }
    }

    internal class RecordedCall<TTarget, TArgs> : RecordedCallWrapped<TTarget>, IRecordedCall<TTarget, TArgs>
        where TTarget : class?
    {
        public RecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall)
        {
            Args = args;
        }
        
        public new TArgs Args { get; }
        CallArguments IRecordedCall.Args => CallInfo.Arguments;
    }
}
