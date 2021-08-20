using DivertR.Core;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : RecordedCallArgs<TTarget>, IFuncRecordedCall<TTarget, TReturn> where TTarget : class
    {
        private readonly IRecordedCall<TTarget, TReturn> _recordedCall;

        internal FuncRecordedCall(IRecordedCall<TTarget, TReturn> recordedCall, ParsedCallExpression parsedCallExpression)
            : base(recordedCall, parsedCallExpression)
        {
            _recordedCall = recordedCall;
        }

        public CallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
        public ICallReturn<TReturn>? Returned => _recordedCall.Returned;

        ICallReturn? IRecordedCall<TTarget>.Returned => Returned;
    }
}