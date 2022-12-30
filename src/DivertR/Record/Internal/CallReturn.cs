using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallReturn
    {
        public static readonly CallReturn None = new();
        
        private readonly Exception? _exception;

        private CallReturn()
        {
            IsReturned = false;
        }
        
        public CallReturn(object? returnValue)
        {
            Value = returnValue;
            IsReturned = true;
        }

        public CallReturn(Exception exception)
        {
            _exception = exception;
            IsReturned = false;
        }
        
        public object? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool IsReturned { get; }

        public bool IsCompleted => !ReferenceEquals(None, this);

        public Exception? RawException => _exception;

        public Exception? Exception
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_exception != null)
                {
                    return _exception;
                }

                if (Value is Task { IsFaulted: true, Exception: { } } value)
                {
                    // Return first aggregate exception to be consistent with await exception behaviour
                    return value.Exception.InnerExceptions.FirstOrDefault();
                }

                return null;
            }
        }
    }
}