﻿using System;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal abstract class RecordedCallArgs<TTarget> : IRecordedCallArgs where TTarget : class
    {
        protected readonly IRecordedCall<TTarget> RecordedCall;
        private readonly ParsedCallExpression _parsedCallExpression;

        internal RecordedCallArgs(IRecordedCall<TTarget> recordedCall, ParsedCallExpression parsedCallExpression)
        {
            RecordedCall = recordedCall;
            _parsedCallExpression = parsedCallExpression;
        }

        public T1 Args<T1>(Action<T1>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1));
            var result = (T1) RecordedCall.Args[0];
            action?.Invoke(result);

            return result;
        }

        public (T1, T2) Args<T1, T2>(Action<T1, T2>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1]);
            action?.Invoke(result.Item1, result.Item2);

            return result;
        }

        public (T1, T2, T3) Args<T1, T2, T3>(Action<T1, T2, T3>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2]);
            action?.Invoke(result.Item1, result.Item2, result.Item3);

            return result;
        }

        public (T1, T2, T3, T4) Args<T1, T2, T3, T4>(Action<T1, T2, T3, T4>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2], (T4) RecordedCall.Args[3]);
            action?.Invoke(result.Item1, result.Item2, result.Item3, result.Item4);

            return result;
        }

        public (T1, T2, T3, T4, T5) Args<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2], (T4) RecordedCall.Args[3], 
                (T5) RecordedCall.Args[4]);
            action?.Invoke(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5);

            return result;
        }

        public (T1, T2, T3, T4, T5, T6) Args<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2], (T4) RecordedCall.Args[3], 
                (T5) RecordedCall.Args[4], (T6) RecordedCall.Args[5]);
            action?.Invoke(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6);

            return result;
        }

        public (T1, T2, T3, T4, T5, T6, T7) Args<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2], (T4) RecordedCall.Args[3], 
                (T5) RecordedCall.Args[4], (T6) RecordedCall.Args[5], (T7) RecordedCall.Args[6]);
            action?.Invoke(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7);

            return result;
        }

        public (T1, T2, T3, T4, T5, T6, T7, T8) Args<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8>? action = null)
        {
            _parsedCallExpression.ValidateArgumentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
            var result = ((T1) RecordedCall.Args[0], (T2) RecordedCall.Args[1], (T3) RecordedCall.Args[2], (T4) RecordedCall.Args[3], 
                (T5) RecordedCall.Args[4], (T6) RecordedCall.Args[5], (T7) RecordedCall.Args[6], (T8) RecordedCall.Args[7]);
            action?.Invoke(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7, result.Item8);

            return result;
        }
    }
}