namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : IFuncRecordedCall<TTarget, TReturn> where TTarget : class
    {
        private readonly IRecordedCall<TTarget> _recordedCall;

        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall)
        {
            _recordedCall = recordedCall;
            
            if (recordedCall.Returned != null)
            {
                Returned = new CallReturn<TReturn>(recordedCall.Returned);
            }
        }

        public CallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
        public CallArguments Args => _recordedCall.CallInfo.Arguments;
        public ICallReturn<TReturn>? Returned { get; }
        ICallReturn? IRecordedCall.Returned => Returned;
    }

    internal class FuncRecordedCall<TTarget, TReturn, TArgs> : FuncRecordedCall<TTarget, TReturn>, IFuncRecordedCall<TTarget, TReturn, TArgs>
        where TTarget : class
    {
        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall)
        {
            Args = args;
        }

        public new TArgs Args { get; }
    }
}