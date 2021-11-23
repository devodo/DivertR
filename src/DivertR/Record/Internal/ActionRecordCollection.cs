using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionRecordCollection<TTarget> : IActionRecordCollection<TTarget> where TTarget : class
    {
        private readonly IReadOnlyCollection<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionRecordCollection(IReadOnlyCollection<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IActionRecordCollection<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            _parsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            
            return new ActionRecordCollection<TTarget, TArgs>(_recordedCalls, _parsedCallExpression, valueTupleFactory);
        }

        public IEnumerator<IActionRecordedCall<TTarget>> GetEnumerator()
        {
            return _recordedCalls.Select(x => new ActionRecordedCall<TTarget>(x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public int Count => _recordedCalls.Count;
    }

    internal class ActionRecordCollection<TTarget, TArgs> : IActionRecordCollection<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IReadOnlyCollection<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly IValueTupleMapper _valueTupleMapper;

        public ActionRecordCollection(IReadOnlyCollection<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleMapper valueTupleMapper)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
            _valueTupleMapper = valueTupleMapper;
        }
        
        public IActionRecordCollection<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRecordCollection<TTarget, TNewArgs>(_recordedCalls, _parsedCallExpression, _valueTupleMapper);
        }

        public IEnumerator<IActionRecordedCall<TTarget, TArgs>> GetEnumerator()
        {
            return _recordedCalls
                .Select(call => new ActionRecordedCall<TTarget, TArgs>(call, (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordedCalls.Count;
    }
}