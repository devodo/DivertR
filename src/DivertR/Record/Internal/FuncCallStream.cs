using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : IFuncCallStream<TTarget, TReturn> where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly RecordedCall<TTarget, TReturn>[] _recordedCalls;

        public FuncCallStream(ParsedCallExpression parsedCallExpression, RecordedCall<TTarget, TReturn>[] recordedCalls)
        {
            _parsedCallExpression = parsedCallExpression;
            _recordedCalls = recordedCalls;
        }
        
        public IReadOnlyList<IRecordedCall<TTarget, TReturn>> Visit(Action<IRecordedCall<TTarget, TReturn>>? visitor = null)
        {
            return VisitItems(_recordedCalls, visitor);
        }
        
        public IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>>? visitor = null)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));
            
            return VisitItems(_recordedCalls.UnsafeCast<IRecordedCall<TTarget, TReturn, T1>>(), visitor);
        }

        public IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>, T1> visitor)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));
            var castCalls = _recordedCalls.UnsafeCast<IRecordedCall<TTarget, TReturn, T1>>();

            foreach (var call in castCalls)
            {
                visitor.Invoke(call, call.Arg1);
            }

            return castCalls;
        }
        
        public IEnumerable<TOut> Visit<T1, TOut>(Func<IRecordedCall<TTarget, TReturn, T1>, T1, TOut> visitor)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));
            var castCalls = _recordedCalls.UnsafeCast<IRecordedCall<TTarget, TReturn, T1>>();

            foreach (var call in castCalls)
            {
                yield return visitor.Invoke(call, call.Arg1);
            }
        }

        public IReadOnlyList<IRecordedCall<TTarget, TReturn, T1, T2>> Visit<T1, T2>(Action<IRecordedCall<TTarget, TReturn, T1, T2>>? visitor = null)
        {
            return VisitItems(_recordedCalls.UnsafeCast<IRecordedCall<TTarget, TReturn, T1, T2>>(), visitor);
        }
        
        private static IReadOnlyList<T> VisitItems<T>(IReadOnlyList<T> items, Action<T>? action = null)
        {
            if (action == null)
            {
                return items;
            }

            foreach (var item in items)
            {
                action.Invoke(item);
            }

            return items;
        }

        public IEnumerator<IRecordedCall<TTarget, TReturn>> GetEnumerator()
        {
            return ((IEnumerable<IRecordedCall<TTarget, TReturn>>) _recordedCalls).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Length;

        public IRecordedCall<TTarget, TReturn> this[int index] => _recordedCalls[index];
    }
}
