using System;
using System.Collections;
using System.Collections.Generic;

namespace DivertR.Redirects
{
    internal class FuncCallStream<TTarget, TReturn> : IFuncCallStream<TTarget, TReturn> where TTarget : class
    {
        private readonly RecordedCall<TTarget, TReturn>[] _recordedCalls;

        public FuncCallStream(RecordedCall<TTarget, TReturn>[] recordedCalls)
        {
            _recordedCalls = recordedCalls;
        }

        public IFuncCallStream<TTarget, TReturn> Visit<T1>(Action<T1, ICallReturn<TReturn>?> verifyDelegate)
        {
            foreach (var recordedCall in _recordedCalls)
            {
                verifyDelegate.Invoke((T1) recordedCall.CallInfo.Arguments[0], recordedCall.Returned);
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
    }
}
