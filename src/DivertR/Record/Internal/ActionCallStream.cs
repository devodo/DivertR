using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : IActionCallStream<TTarget> where TTarget : class
    {
        private readonly ActionRecordedCall<TTarget>[] _recordedCalls;

        public ActionCallStream(ActionRecordedCall<TTarget>[] recordedCalls)
        {
            _recordedCalls = recordedCalls;
        }

        public IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>> visitor)
        {
            foreach (var recordedCall in _recordedCalls)
            {
                visitor.Invoke(recordedCall);
            }

            return this;
        }

        public IActionCallStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>, int> visitor)
        {
            for (var i = 0; i < _recordedCalls.Length; i++)
            {
                visitor.Invoke(_recordedCalls[i], i);
            }
            
            return this;
        }

        public IEnumerator<IActionRecordedCall<TTarget>> GetEnumerator()
        {
            return ((IEnumerable<IActionRecordedCall<TTarget>>) _recordedCalls).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Length;

        public IActionRecordedCall<TTarget> this[int index] => _recordedCalls[index];
    }
}
