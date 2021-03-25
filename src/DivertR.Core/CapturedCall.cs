namespace DivertR.Core
{
    public class CapturedCall<T> where T : class
    {
        public CallInfo CallInfo { get; }

        public object? ReturnValue { get; }
        public T Proxy { get; }
        public T? Original { get; }

        public CapturedCall(CallInfo callInfo, object? returnValue, T proxy, T? original)
        {
            CallInfo = callInfo;
            ReturnValue = returnValue;
            Proxy = proxy;
            Original = original;
        }
    }
}