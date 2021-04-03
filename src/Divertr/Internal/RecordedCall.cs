using DivertR.Core;

namespace DivertR.Internal
{
    internal class RecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
        public CallInfo<TTarget> CallInfo { get; }

        public object? ReturnValue { get; set; }

        public RecordedCall(CallInfo<TTarget> callInfo)
        {
            CallInfo = callInfo;
        }
    }
}