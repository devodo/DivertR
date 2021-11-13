namespace DivertR.Record.Internal
{
    internal class ActionRecordedCall<TTarget> : IActionRecordedCall<TTarget> where TTarget : class
    {
        private readonly IRecordedCall<TTarget> _recordedCall;

        public ActionRecordedCall(IRecordedCall<TTarget> recordedCall)
        {
            _recordedCall = recordedCall;
        }

        public CallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
        public CallArguments Args => _recordedCall.CallInfo.Arguments;
        public ICallReturn? Returned => _recordedCall.Returned;
    }
    
    internal class ActionRecordedCall<TTarget, TArgs> : ActionRecordedCall<TTarget>, IActionRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
        public ActionRecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall)
        {
            Args = args;
        }

        public new TArgs Args { get; }
    }
}