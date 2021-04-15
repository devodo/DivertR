using DivertR.Core;

namespace DivertR.Redirects
{
    public class RecordedCall<TTarget> where TTarget : class
    {
        internal readonly CallReturn CallReturn;
        public CallInfo<TTarget> CallInfo { get; }

        public object? ReturnObject => CallReturn.ReturnedObject;
        public bool CallReturned => CallReturn.IsSet;

        internal RecordedCall(CallInfo<TTarget> callInfo, CallReturn callReturn)
        {
            CallReturn = callReturn;
            CallInfo = callInfo;
        }
    }

    public class RecordedCall<TTarget, TReturn> : RecordedCall<TTarget> where TTarget : class
    {
        internal RecordedCall(CallInfo<TTarget> callInfo, CallReturn callReturn) : base(callInfo, callReturn)
        {
        }

        public TReturn ReturnValue
        {
            get
            {
                if (ReturnObject == null)
                {
                    return default!;
                }
                
                return (TReturn) ReturnObject;
            }
        }
    }
}