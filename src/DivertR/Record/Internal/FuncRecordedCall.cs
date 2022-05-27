namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : RecordedCallInternal<TTarget>, IFuncRecordedCall<TTarget, TReturn> where TTarget : class
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
    
    internal class FuncRecordedCall<TTarget, TArgs, TReturn> : RecordedCall<TTarget, TArgs>, IFuncRecordedCall<TTarget, TArgs, TReturn> where TTarget : class
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
}