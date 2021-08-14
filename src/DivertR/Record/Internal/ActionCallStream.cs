using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : IActionCallStream<TTarget> where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly RecordedCall<TTarget>[] _recordedCalls;

        public ActionCallStream(ParsedCallExpression parsedCallExpression, RecordedCall<TTarget>[] recordedCalls)
        {
            _parsedCallExpression = parsedCallExpression;
            _recordedCalls = recordedCalls;
        }
        
        public IReadOnlyList<IRecordedCall<TTarget>> Visit(Action<IRecordedCall<TTarget>>? visitor = null)
        {
            return VisitItems(_recordedCalls, visitor);
        }
        
        public IReadOnlyList<IRecordedCall<TTarget, T1>> Visit<T1>(Action<IRecordedCall<TTarget, T1>>? visitor = null)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));
            
            return VisitItems(_recordedCalls.UnsafeCast<IRecordedCall<TTarget, T1>>(), visitor);
        }

        public IReadOnlyList<IRecordedCall<TTarget, T1>> Visit<T1>(Action<IRecordedCall<TTarget, T1>, T1> visitor)
        {
            _parsedCallExpression.ValidateArguments(typeof(T1));
            var castCalls = _recordedCalls.UnsafeCast<IRecordedCall<TTarget, T1>>();

            foreach (var call in castCalls)
            {
                visitor.Invoke(call, default!);
            }

            return castCalls;
        }

        public IReadOnlyList<IRecordedCall<TTarget, T1, T2>> Visit<T1, T2>(Action<IRecordedCall<TTarget, T1, T2>>? visitor = null)
        {
            return VisitItems(_recordedCalls.UnsafeCast<IRecordedCall<TTarget, T1, T2>>(), visitor);
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

        public IEnumerator<IRecordedCall<TTarget>> GetEnumerator()
        {
            return ((IEnumerable<IRecordedCall<TTarget>>) _recordedCalls).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Length;

        public IRecordedCall<TTarget> this[int index] => _recordedCalls[index];
    }
}
