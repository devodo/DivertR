using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : CallStream<IFuncRecordedCall<TTarget, TReturn>, TTarget>, IFuncCallStream<TTarget, TReturn>
        where TTarget : class
    {
        protected readonly ICallValidator CallValidator;
        
        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn>> recordedCalls, ICallValidator callValidator)
            : base(recordedCalls)
        {
            CallValidator = callValidator;
        }
        
        public IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);
            
            return new FuncCallStream<TTarget, TReturn, TArgs>(mappedCall, CallValidator);
        }
        
        public IReplayResult Replay<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public IReplayResult Replay<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, int, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        public Task<IReplayResult> Replay<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, int, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Replay(visitor);
        }

        internal static IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> MapCalls<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
        {
            return calls.Select(call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }
    }

    internal class FuncCallStream<TTarget, TReturn, TArgs> : CallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>, TTarget, TArgs>, IFuncCallStream<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        protected readonly ICallValidator CallValidator;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> recordedCalls, ICallValidator callValidator)
            : base(recordedCalls)
        {
            CallValidator = callValidator;
        }

        public IFuncCallStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = FuncCallStream<TTarget, TReturn>.MapCalls<TNewArgs>(Calls, valueTupleFactory);
            
            return new FuncCallStream<TTarget, TReturn, TNewArgs>(mappedCall, CallValidator);
        }
    }
}