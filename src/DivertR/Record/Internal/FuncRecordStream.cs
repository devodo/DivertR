using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncRecordStream<TTarget, TReturn> : IFuncRecordStream<TTarget, TReturn> where TTarget : class
    {
        private readonly IEnumerable<IFuncRecordedCall<TTarget, TReturn>> _recordedCalls;
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public IFuncRecordStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRecordStream<TTarget, TReturn, TArgs>(_recordedCalls, _parsedCallExpression);
        }
        
        public FuncRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, bool skipValidation = false)
        {
            if (!skipValidation)
            {
                parsedCallExpression.Validate(typeof(TReturn), Array.Empty<Type>());
            }
            
            _recordedCalls = recordedCalls.Select(x => new FuncRecordedCall<TTarget, TReturn>(x));
            _parsedCallExpression = parsedCallExpression;
        }

        public IEnumerable<IFuncRecordedCall<TTarget, TReturn>> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>> visitor)
        {
            return ((IEnumerable<IFuncRecordedCall<TTarget, TReturn>>) this).ForEach(visitor);
        }

        public IEnumerable<IFuncRecordedCall<TTarget, TReturn>> ForEach(Action<IFuncRecordedCall<TTarget, TReturn>, int> visitor)
        {
            return ((IEnumerable<IFuncRecordedCall<TTarget, TReturn>>) this).ForEach(visitor);
        }

        public IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().ForEach(visitor);
        }

        public IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> ForEach<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().ForEach(visitor);
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn>> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
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
        private readonly ParsedCallExpression _parsedCallExpression;
        private readonly IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> _recordedCalls;

        public FuncRecordStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, bool skipValidation = false)
        {
            var valueTupleFactory = ValueTupleFactory.CreateFactory<TArgs>();

            if (!skipValidation)
            {
                parsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            }
            
            _parsedCallExpression = parsedCallExpression;
            _recordedCalls = recordedCalls.Select(call => new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleFactory.Create(call.Args)));
        }

        public IFuncRecordStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new FuncRecordStream<TTarget, TReturn, TNewArgs>(_recordedCalls, _parsedCallExpression);
        }

        public IEnumerator<IFuncRecordedCall<TTarget, TReturn, TArgs>> GetEnumerator()
        {
            return _recordedCalls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}