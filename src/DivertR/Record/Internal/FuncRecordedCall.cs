namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : RecordedCallInternal<TTarget>, IFuncRecordedCall<TTarget, TReturn> where TTarget : class?
    {
        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall) : base(recordedCall)
        {
            if (recordedCall.Returned != null)
            {
                Returned = new CallReturn<TReturn>(recordedCall.Returned);
            }
        }

        public new ICallReturn<TReturn>? Returned { get; }
    }
    
    internal class FuncRecordedCall<TTarget, TReturn, TArgs> : RecordedCall<TTarget, TArgs>, IFuncRecordedCall<TTarget, TReturn, TArgs> where TTarget : class?
    {
        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall, args)
        {
            if (recordedCall.Returned != null)
            {
                Returned = new CallReturn<TReturn>(recordedCall.Returned);
            }
        }

        public new ICallReturn<TReturn>? Returned { get; }
    }
    
    internal class FuncRecordedCall<TReturn> : IFuncRecordedCall<TReturn>
    {
        private readonly IRecordedCall _recordedCall;

        public FuncRecordedCall(IRecordedCall recordedCall)
        {
            _recordedCall = recordedCall;
            
            if (recordedCall.Returned != null)
            {
                Returned = new CallReturn<TReturn>(recordedCall.Returned);
            }
        }

        public ICallInfo CallInfo => _recordedCall.CallInfo;
        public CallArguments Args => _recordedCall.Args;
        
        ICallReturn? IRecordedCall.Returned => Returned;

        public ICallReturn<TReturn>? Returned { get; }
    }
}
