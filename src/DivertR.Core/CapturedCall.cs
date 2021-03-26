namespace DivertR.Core
{
    public class CapturedCall<T> where T : class
    {
        public CallInfo<T> CallInfo { get; }

        public object? ReturnValue { get; }

        public CapturedCall(CallInfo<T> callInfo, object? returnValue)
        {
            CallInfo = callInfo;
            ReturnValue = returnValue;
        }
    }
}