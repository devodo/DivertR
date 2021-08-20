using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : IFuncCallStream<TTarget, TReturn> where TTarget : class
    {
        private readonly FuncRecordedCall<TTarget, TReturn>[] _recordedCalls;

        public FuncCallStream(FuncRecordedCall<TTarget, TReturn>[] recordedCalls)
        {
            _recordedCalls = recordedCalls;
        }

        public IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>> visitor)
        {
            foreach (var recordedCall in _recordedCalls)
            {
                visitor.Invoke(recordedCall);
            }

            return this;
        }

        public IFuncCallStream<TTarget, TReturn> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>, int> visitor)
        {
            for (var i = 0; i < _recordedCalls.Length; i++)
            {
                visitor.Invoke(_recordedCalls[i], i);
            }
            
            return this;
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn>> GetEnumerator()
        {
            return ((IEnumerable<IFuncRecordedCall<TTarget, TReturn>>) _recordedCalls).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Length;

        public IFuncRecordedCall<TTarget, TReturn> this[int index] => _recordedCalls[index];
    }
}
