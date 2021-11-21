using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncRecordStream<TTarget, TReturn> : IFuncRecordStream<TTarget, TReturn> where TTarget : class
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public FuncRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IFuncRecordStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();
            _parsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            
            return new FuncRecordStream<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression, valueTupleFactory);
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

    internal class FuncRecordStream<TTarget, TReturn, TArgs> : IFuncRecordStream<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IEnumerable<IRecordedCall<TTarget>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly IValueTupleFactory _valueTupleFactory;


        public FuncRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleFactory valueTupleFactory)
        {
            _recordedCalls = recordedCalls;
            _parsedCallExpression = parsedCallExpression;
            _valueTupleFactory = valueTupleFactory;
        }

        public IFuncRecordStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRecordStream<TTarget, TReturn, TNewArgs>(_recordedCalls, _parsedCallExpression, _valueTupleFactory);
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn, TArgs>> GetEnumerator()
        {
            return _recordedCalls
                .Select(call => new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) _valueTupleFactory.Create(call.Args.InternalArgs)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}