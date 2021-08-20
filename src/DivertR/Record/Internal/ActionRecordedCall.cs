using DivertR.Core;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionRecordedCall<TTarget> : RecordedCallArgs<TTarget>, IActionRecordedCall<TTarget> where TTarget : class
    {
        private readonly IRecordedCall<TTarget> _recordedCall;

        internal ActionRecordedCall(IRecordedCall<TTarget> recordedCall, ParsedCallExpression parsedCallExpression)
            : base(recordedCall, parsedCallExpression)
        {
            _recordedCall = recordedCall;
        }

        public CallInfo<TTarget> CallInfo => _recordedCall.CallInfo;
        public ICallReturn? Returned => _recordedCall.Returned;
    }
}