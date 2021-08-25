using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionRecordedCall<TTarget> : RecordedCallArgs<TTarget>, IActionRecordedCall<TTarget> where TTarget : class
    {
        internal ActionRecordedCall(IRecordedCall<TTarget> recordedCall, ParsedCallExpression parsedCallExpression)
            : base(recordedCall, parsedCallExpression)
        {
        }

        public CallInfo<TTarget> CallInfo => RecordedCall.CallInfo;
        public CallArguments Args => RecordedCall.CallInfo.Arguments;
        public ICallReturn? Returned => RecordedCall.Returned;
    }
}