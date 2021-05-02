using System;

namespace DivertR.Redirects
{
    internal class CallReturn : ICallReturn
    {
        public object? Value { get; }
        
        public Exception? Exception { get; }
        
        protected CallReturn(object? returnValue, Exception? exception)
        {
            Value = returnValue;
            Exception = exception;
        }
    }
    
    internal class CallReturn<TReturn> : CallReturn, ICallReturn<TReturn>
    {
        public new TReturn Value
        {
            get
            {
                if (base.Value == null)
                {
                    return default!;
                }

                return (TReturn) base.Value;
            }
        }

        public CallReturn(TReturn returnValue, Exception? exception)
            : base(returnValue, exception)
        {
        }
    }
}
