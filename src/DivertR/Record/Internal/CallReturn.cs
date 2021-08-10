using System;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallReturn : ICallReturn
    {
        private readonly Exception? _exception;
        public object? Value { get; }

        public Exception? Exception
        {
            get
            {
                if (_exception != null)
                {
                    return _exception;
                }

                if (Value is Task {IsFaulted: true, Exception: { }} value)
                {
                    return value.Exception.InnerExceptions.Count > 1
                        ? value.Exception
                        : value.Exception.InnerException;
                }

                return null;
            }
        }
        
        protected CallReturn(object? returnValue, Exception? exception)
        {
            _exception = exception;
            Value = returnValue;
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
