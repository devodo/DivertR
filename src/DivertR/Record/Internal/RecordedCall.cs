using System;
using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal class RecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
        private readonly object _returnedLock = new object();
        
        private ICallReturn? _callReturn;
        
        public RecordedCall(CallInfo<TTarget> callInfo)
        {
            CallInfo = callInfo;
        }
        
        public CallInfo<TTarget> CallInfo { get; }

        public CallArguments Args => CallInfo.Arguments;
        
        public ICallReturn? Returned
        {
            get
            {
                lock (_returnedLock)
                {
                    return _callReturn;
                }
            }

            private set
            {
                lock (_returnedLock)
                {
                    _callReturn = value;
                }
            }
        }
        
        public void SetReturned(object? returnedObject)
        {
            Returned = new CallReturn(returnedObject, null);
        }

        public void SetException(Exception exception)
        {
            Returned = new CallReturn(null, exception);
        }
    }
}