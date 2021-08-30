using System;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallReturn : ICallReturn
    {
        private readonly Exception? _exception;
        public object? Value { get; }
        
        public CallReturn(object? returnValue, Exception? exception)
        {
            _exception = exception;
            Value = returnValue;
        }

        public Exception? Exception
        {
            get
            {
                if (_exception != null)
                {
                    return _exception;
                }

                if (Value is Task { IsFaulted: true, Exception: { } } value)
                {
                    return value.Exception.InnerExceptions.Count > 1
                        ? value.Exception
                        : value.Exception.InnerException;
                }

                return null;
            }
        }
    }
    
    internal class CallReturn<TReturn> : ICallReturn<TReturn>
    {
        private readonly ICallReturn _callReturn;

        public CallReturn(ICallReturn callReturn)
        {
            _callReturn = callReturn;
        }

        public TReturn Value => (TReturn) _callReturn.Value!;
        object? ICallReturn.Value => Value;
        public Exception? Exception => _callReturn.Exception;
    }
}