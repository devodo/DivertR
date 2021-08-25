using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : RecordedCallArgs<TTarget>, IFuncRecordedCall<TTarget, TReturn> where TTarget : class
    {
        internal FuncRecordedCall(IRecordedCall<TTarget> recordedCall, ParsedCallExpression parsedCallExpression)
            : base(recordedCall, parsedCallExpression)
        {
            if (recordedCall.Returned != null)
            {
                Returned = new CallReturn<TReturn>(recordedCall.Returned);
            }
        }

        public CallInfo<TTarget> CallInfo => RecordedCall.CallInfo;
        public CallArguments Args => RecordedCall.CallInfo.Arguments;
        public ICallReturn<TReturn>? Returned { get; }
        ICallReturn? IRecordedCall<TTarget>.Returned => Returned;
    }
}