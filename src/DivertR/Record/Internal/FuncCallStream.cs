using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : CallStream<IFuncRecordedCall<TTarget, TReturn>>, IFuncCallStream<TTarget, TReturn>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public FuncCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(MapCalls(recordedCalls))
        {
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            _parsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            
            return new FuncCallStream<TTarget, TReturn, TArgs>(Calls, _parsedCallExpression, valueTupleFactory);
        }
        
        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }

        private static IEnumerable<IFuncRecordedCall<TTarget, TReturn>> MapCalls(IEnumerable<IRecordedCall<TTarget>> calls)
        {
            return calls.Select(call => new FuncRecordedCall<TTarget, TReturn>(call));
        }
    }

    internal class FuncCallStream<TTarget, TReturn, TArgs> : CallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>, IFuncCallStream<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleMapper valueTupleMapper)
            : base(MapCalls(recordedCalls, valueTupleMapper))
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IFuncCallStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            _parsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            
            return new FuncCallStream<TTarget, TReturn, TNewArgs>(Calls, _parsedCallExpression, valueTupleFactory);
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn>, TArgs> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn>, TArgs, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }

        private static IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> MapCalls(IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
        {
            return calls.Select(call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }
}