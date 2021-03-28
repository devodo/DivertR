using DivertR.Core;

namespace DivertR.Internal
{
    internal class RecordedCall<T> : IRecordedCall<T> where T : class
    {
        public CallInfo<T> CallInfo { get; }

        public object? ReturnValue { get; set; }

        public RecordedCall(CallInfo<T> callInfo)
        {
            CallInfo = callInfo;
        }
    }
}