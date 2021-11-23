using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionRecordEnumerable<TTarget> : IActionRecordEnumerable<TTarget> where TTarget : class
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionRecordEnumerable(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IActionRecordEnumerable<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            _parsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            
            return new ActionRecordEnumerable<TTarget, TArgs>(_recordedCalls, _parsedCallExpression, valueTupleFactory);
        }

        public IEnumerator<IActionRecordedCall<TTarget>> GetEnumerator()
        {
            return _recordedCalls.Select(x => new ActionRecordedCall<TTarget>(x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class ActionRecordEnumerable<TTarget, TArgs> : IActionRecordEnumerable<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly IValueTupleMapper _valueTupleMapper;

        public ActionRecordEnumerable(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleMapper valueTupleMapper)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
            _valueTupleMapper = valueTupleMapper;
        }
        
        public IActionRecordEnumerable<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new ActionRecordEnumerable<TTarget, TNewArgs>(_recordedCalls, _parsedCallExpression, _valueTupleMapper);
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
    }
}