using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : IFuncCallStream<TTarget, TReturn> where TTarget : class
    {
        private readonly FuncRecordedCall<TTarget, TReturn>[] _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(FuncRecordedCall<TTarget, TReturn>[] recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
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

        public IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncCallStream<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression);
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var callStream = new FuncCallStream<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression);
            return callStream.ForEach(visitor);
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var callStream = new FuncCallStream<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression);
            return callStream.ForEach(visitor);
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

    internal class FuncCallStream<TTarget, TReturn, TArgs> : IFuncCallStream<TTarget, TReturn, TArgs> where TTarget : class
    {
        private readonly FuncRecordedCall<TTarget, TReturn>[] _recordedCalls;
        private readonly IValueTupleFactory _valueTupleFactory;
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(FuncRecordedCall<TTarget, TReturn>[] recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
            
            _valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();
            _parsedCallExpression.Validate(typeof(TReturn), _valueTupleFactory.ArgumentTypes, false);
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn, TArgs>> GetEnumerator()
        {
            foreach (var call in _recordedCalls)
            {
                yield return new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) _valueTupleFactory.Create(call.Args));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Length;

        public IFuncRecordedCall<TTarget, TReturn, TArgs> this[int index]
        {
            get
            {
                return new FuncRecordedCall<TTarget, TReturn, TArgs>(_recordedCalls[index], (TArgs) _valueTupleFactory.Create(_recordedCalls[index].Args));
            }
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> ForEach(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor)
        {
            foreach (var call in this)
            {
                visitor.Invoke(call);
            }

            return this;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> ForEach(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor)
        {
            var count = 0;

            foreach (var call in this)
            {
                visitor.Invoke(call, count++);
            }

            return this;
        }
    }
}
