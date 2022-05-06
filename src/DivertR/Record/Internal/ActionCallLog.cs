using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallLog<TTarget> : ActionCallStream<TTarget>, IActionCallLog<TTarget> where TTarget : class
    {
        public ActionCallLog(IReadOnlyCollection<IRecordedCall<TTarget>> calls, ICallValidator callValidator)
            : base(calls, callValidator)
        {
        }
        
        public new ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper)
        {
            var mappedCalls = new MappedCollection<IRecordedCall<TTarget>, TMap>(this, mapper);
            return new CallLog<TMap>(mappedCalls);
        }

        public new ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper)
        {
            var mappedCalls = new MappedCollection<IRecordedCall<TTarget>, TMap>(this, call => mapper.Invoke(call, call.Args));
            return new CallLog<TMap>(mappedCalls);
        }
        
        public new IActionCallLog<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);

            return new ActionCallLog<TTarget, TArgs>(mappedCall, CallValidator);
        }
        
        public int Count => Calls.Count;
        
        internal static IReadOnlyCollection<IRecordedCall<TTarget, TArgs>> MapCalls<TArgs>(IReadOnlyCollection<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
            where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return new MappedCollection<IRecordedCall<TTarget>, IRecordedCall<TTarget, TArgs>>(calls, call =>
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }

        private new IReadOnlyCollection<IRecordedCall<TTarget>> Calls => (IReadOnlyCollection<IRecordedCall<TTarget>>) base.Calls;
    }

    internal class ActionCallLog<TTarget, TArgs> : ActionCallStream<TTarget, TArgs>, IActionCallLog<TTarget, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public ActionCallLog(IReadOnlyCollection<IRecordedCall<TTarget, TArgs>> recordedCalls, ICallValidator callValidator)
            : base(recordedCalls, callValidator)
        {
        }

        public new ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget, TArgs>, TMap>(Calls, mapper);
            return new CallLog<TMap>(mappedCollection);
        }

        public new ICallLog<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IRecordedCall<TTarget, TArgs>, TMap>(Calls, call => mapper.Invoke(call, call.Args));
            return new CallLog<TMap>(mappedCollection);
        }
        
        public new IActionCallLog<TTarget, TNewArgs> WithArgs<TNewArgs>()
            where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            CallValidator.Validate(valueTupleFactory);
            var mappedCall = ActionCallLog<TTarget>.MapCalls<TNewArgs>(Calls, valueTupleFactory);

            return new ActionCallLog<TTarget, TNewArgs>(mappedCall, CallValidator);
        }
        
        public int Count => Calls.Count;

        private new IReadOnlyCollection<IRecordedCall<TTarget, TArgs>> Calls => (IReadOnlyCollection<IRecordedCall<TTarget, TArgs>>) base.Calls;
    }
}