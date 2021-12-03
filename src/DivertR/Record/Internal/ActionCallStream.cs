using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : CallStream<IRecordedCall<TTarget>>, IActionCallStream<TTarget> where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls)
        {
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IActionCallStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            _parsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            
            return new ActionCallStream<TTarget, TArgs>(Calls, _parsedCallExpression, valueTupleFactory);
        }

        public int Replay(Action<IRecordedCall<TTarget>, CallArguments> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IRecordedCall<TTarget>, CallArguments, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, CallArguments, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }
    }

    internal class ActionCallStream<TTarget, TArgs> : CallStream<IRecordedCall<TTarget, TArgs>>, IActionCallStream<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression, IValueTupleMapper valueTupleMapper)
            : base(MapCalls(recordedCalls, valueTupleMapper))
        {
            _parsedCallExpression = parsedCallExpression;
        }
        
        public IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            _parsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            
            return new ActionCallStream<TTarget, TNewArgs>(Calls, _parsedCallExpression, valueTupleFactory);
        }

        public int Replay(Action<IRecordedCall<TTarget>, TArgs> visitor)
        {
            return Calls.Select(call =>
            {
                visitor.Invoke(call, call.Args);

                return call;
            }).Count();
        }

        public int Replay(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor)
        {
            return Calls.Select((call, i) =>
            {
                visitor.Invoke(call, call.Args, i);

                return call;
            }).Count();
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget>, TArgs, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args);
                count++;
            }

            return count;
        }

        public async Task<int> Replay(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor)
        {
            var count = 0;
            
            foreach (var call in Calls)
            {
                await visitor.Invoke(call, call.Args, count++);
            }

            return count;
        }

        private static IEnumerable<IRecordedCall<TTarget, TArgs>> MapCalls(
            IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
        {
            return calls.Select(call =>
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }
}