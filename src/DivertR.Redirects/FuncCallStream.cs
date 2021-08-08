using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivertR.Redirects
{
    internal class FuncCallStream<TTarget, TReturn> : IFuncCallStream<TTarget, TReturn> where TTarget : class
    {
        private readonly RecordedCall<TTarget, TReturn>[] _recordedCalls;

        public FuncCallStream(RecordedCall<TTarget, TReturn>[] recordedCalls)
        {
            _recordedCalls = recordedCalls;
        }

        public IReadOnlyList<IRecordedCall<TTarget, TReturn, T1>> Visit<T1>(Action<IRecordedCall<TTarget, TReturn, T1>>? visitor = null)
        {
            var castCalls = _recordedCalls.Cast<IRecordedCall<TTarget, TReturn, T1>>().ToArray();
            
            if (visitor != null)
            {
                foreach (var recordedCall in castCalls)
                {
                    visitor.Invoke(recordedCall);
                }
            }
            
            return castCalls;
        }

        public async Task<IFuncCallStream<TTarget, TReturn>> Visit<T1>(Func<IRecordedCall<TTarget, TReturn, T1>, Task> visitor)
        {
            foreach (var recordedCall in _recordedCalls)
            {
                await visitor.Invoke((IRecordedCall<TTarget, TReturn, T1>) recordedCall);
            }
            
            return this;
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
