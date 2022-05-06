using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : CallStream<IRecordedCall<TTarget>, TTarget>, IActionCallStream<TTarget> where TTarget : class
    {
        protected readonly ICallValidator CallValidator;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> recordedCalls, ICallValidator callValidator)
            : base(recordedCalls)
        {
            CallValidator = callValidator;
        }
        
        public IActionCallStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);

            return new ActionCallStream<TTarget, TArgs>(mappedCall, CallValidator);
        }

        public IReplayResult Replay<TArgs>(Action<IRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IRecordedCall<TTarget, TArgs>, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IRecordedCall<TTarget, TArgs>, TArgs, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IRecordedCall<TTarget, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IRecordedCall<TTarget, TArgs>, int, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IRecordedCall<TTarget, TArgs>, TArgs, int, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        internal static IEnumerable<IRecordedCall<TTarget, TArgs>> MapCalls<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return calls.Select(call =>
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }

    internal class ActionCallStream<TTarget, TArgs> : CallStream<IRecordedCall<TTarget, TArgs>, TTarget, TArgs>, IActionCallStream<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        protected readonly ICallValidator CallValidator;

        public ActionCallStream(IEnumerable<IRecordedCall<TTarget, TArgs>> recordedCalls, ICallValidator callValidator)
            : base(recordedCalls)
        {
            CallValidator = callValidator;
        }
        
        public IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = ActionCallStream<TTarget>.MapCalls<TNewArgs>(Calls, valueTupleFactory);

            return new ActionCallStream<TTarget, TNewArgs>(mappedCall, CallValidator);
        }
    }
}