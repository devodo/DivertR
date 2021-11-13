using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionRecordStream<TTarget> : IActionRecordStream<TTarget> where TTarget : class
    {
        private readonly IEnumerable<IActionRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, bool skipValidation = false)
        {
            if (!skipValidation)
            {
                parsedCallExpression.Validate(typeof(void), Array.Empty<Type>());
            }
            
            _recordedCalls = recordedCalls.Select(x => new ActionRecordedCall<TTarget>(x));
            _parsedCallExpression = parsedCallExpression;
        }

        public IActionRecordStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>> visitor)
        {
            foreach (var recordedCall in _recordedCalls)
            {
                visitor.Invoke(recordedCall);
            }

            return this;
        }

        public IActionRecordStream<TTarget> ForEach(Action<IActionRecordedCall<TTarget>, int> visitor)
        {
            var count = 0;

            foreach (var call in _recordedCalls)
            {
                visitor.Invoke(call, count++);
            }

            return this;
        }

        public IActionRecordStream<TTarget, TArgs> ForEach<TArgs>(Action<IActionRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            throw new NotImplementedException();
        }

        public IActionRecordStream<TTarget, TArgs> ForEach<TArgs>(Action<IActionRecordedCall<TTarget, TArgs>, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            throw new NotImplementedException();
        }

        public IActionRecordStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRecordStream<TTarget, TArgs>(_recordedCalls, _parsedCallExpression);
        }

        public IEnumerator<IActionRecordedCall<TTarget>> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class ActionRecordStream<TTarget, TArgs> : IActionRecordStream<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IEnumerable<IActionRecordedCall<TTarget, TArgs>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, bool skipValidation = false)
        {
            _parsedCallExpression = parsedCallExpression;
            var valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();

            if (!skipValidation)
            {
                parsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            }
            
            _recordedCalls = recordedCalls.Select(call => new ActionRecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleFactory.Create(call.Args)));
        }
        
        public IActionRecordStream<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRecordStream<TTarget, TNewArgs>(_recordedCalls, _parsedCallExpression);
        }

        public IEnumerator<IActionRecordedCall<TTarget, TArgs>> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
