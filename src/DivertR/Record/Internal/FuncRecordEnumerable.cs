using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncRecordEnumerable<TTarget, TReturn> : IFuncRecordEnumerable<TTarget, TReturn> where TTarget : class
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public FuncRecordEnumerable(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IFuncRecordEnumerable<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            _parsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            
            return new FuncRecordEnumerable<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression, valueTupleFactory);
        }
        
        public IEnumerator<IFuncRecordedCall<TTarget, TReturn>> GetEnumerator()
        {
            return _recordedCalls.Select(x => new FuncRecordedCall<TTarget, TReturn>(x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class FuncRecordEnumerable<TTarget, TReturn, TArgs> : IFuncRecordEnumerable<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly IValueTupleMapper _valueTupleMapper;


        public FuncRecordEnumerable(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleMapper valueTupleMapper)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
            _valueTupleMapper = valueTupleMapper;
        }

        public IFuncRecordEnumerable<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRecordEnumerable<TTarget, TReturn, TNewArgs>(_recordedCalls, _parsedCallExpression, _valueTupleMapper);
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn, TArgs>> GetEnumerator()
        {
            return _recordedCalls
                .Select(call => new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}