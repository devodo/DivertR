using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DivertR.Record.Internal
{
    internal class CallReturn : ICallReturn
    {
        private readonly Exception? _exception;

        public bool IsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !IsException;
        }

        public bool IsException
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public object? Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public CallReturn(object? returnValue, Exception? exception)
        {
            _exception = exception;
            Value = returnValue;
            IsException = exception != null;
        }

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

        public bool IsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _callReturn.IsValue;
        }

        public bool IsException
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _callReturn.IsException;
        }

        public TReturn Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (TReturn) _callReturn.Value!;
        }

        object? ICallReturn.Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value;
        }

        public Exception? Exception
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _callReturn.Exception;
        }
    }
}