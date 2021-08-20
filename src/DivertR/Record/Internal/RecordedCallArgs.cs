using System;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class RecordedCallArgs<TTarget> : IRecordedCallArgs where TTarget : class
    {
        private readonly IRecordedCall<TTarget> _recordedCall;
        private readonly ParsedCallExpression _parsedCallExpression;

        internal RecordedCallArgs(IRecordedCall<TTarget> recordedCall, ParsedCallExpression parsedCallExpression)
        {
            _recordedCall = recordedCall;
            _parsedCallExpression = parsedCallExpression;
        }

        public T1 Args<T1>(Action<T1>? action = null)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));

            return (T1) _recordedCall.CallInfo.Arguments[0];
        }

        public (T1, T2) Args<T1, T2>(Action<T1, T2>? action = null)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1), typeof(T2));

            return ((T1) _recordedCall.CallInfo.Arguments[0], (T2) _recordedCall.CallInfo.Arguments[1]);
        }
    }
}