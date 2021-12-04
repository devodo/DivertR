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
        protected readonly ParsedCallExpression ParsedCallExpression;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls)
        {
            ParsedCallExpression = parsedCallExpression;
        }
        
        public IActionCallStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            ParsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);

            return new ActionCallStream<TTarget, TArgs>(mappedCall, ParsedCallExpression);
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

        internal static IEnumerable<IRecordedCall<TTarget, TArgs>> MapCalls<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return calls.Select(call =>
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }

    internal class ActionCallStream<TTarget, TArgs> : CallStream<IRecordedCall<TTarget, TArgs>>, IActionCallStream<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        protected readonly ParsedCallExpression ParsedCallExpression;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget, TArgs>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls)
        {
            ParsedCallExpression = parsedCallExpression;
        }
        
        public IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            ParsedCallExpression.Validate(typeof(void), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = ActionCallStream<TTarget>.MapCalls<TNewArgs>(Calls, valueTupleFactory);

            return new ActionCallStream<TTarget, TNewArgs>(mappedCall, ParsedCallExpression);
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
    }
}