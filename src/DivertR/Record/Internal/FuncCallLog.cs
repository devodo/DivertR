using System;
using System.Collections;
using System.Collections.Generic;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallLog<TTarget, TReturn> : FuncCallStream<TTarget, TReturn>, IFuncCallLog<TTarget, TReturn> where TTarget : class
    {
        public FuncCallLog(IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls, parsedCallExpression)
        {
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn>, TMap>(Calls, mapper);
            
            return new CallLog<TMap>(mappedCollection);
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn>, TMap>(Calls, call => mapper.Invoke(call, call.Args));
            
            return new CallLog<TMap>(mappedCollection);
        }
        
        public new IFuncCallLog<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = MapCalls<TArgs>(Calls, valueTupleFactory);
            
            return new FuncCallLog<TTarget, TReturn, TArgs>(mappedCall, ParsedCallExpression);
        }
        
        public int Count => Calls.Count;
        
        internal static IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>> MapCalls<TArgs>(IReadOnlyCollection<IRecordedCall<TTarget>> calls, IValueTupleMapper valueTupleMapper)
        {
            return new MappedCollection<IRecordedCall<TTarget>, IFuncRecordedCall<TTarget, TReturn, TArgs>>(calls, call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.Args.InternalArgs)));
        }

        private new IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn>> Calls => (IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn>>) base.Calls;
    }

    internal class FuncCallLog<TTarget, TReturn, TArgs> : FuncCallStream<TTarget, TReturn, TArgs>, IFuncCallLog<TTarget, TReturn, TArgs>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        public FuncCallLog(IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>> recordedCalls, ParsedCallExpression parsedCallExpression)
            : base(recordedCalls, parsedCallExpression)
        {
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap>(Calls, mapper);
            
            return new CallLog<TMap>(mappedCollection);
        }

        public ICallLog<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper)
        {
            var mappedCollection = new MappedCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap>(Calls,
                call => mapper.Invoke(call, call.Args));
            
            return new CallLog<TMap>(mappedCollection);
        }
        
        public new IFuncCallLog<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleFactory = ValueTupleMapperFactory.Create<TNewArgs>();
            ParsedCallExpression.Validate(typeof(TReturn), valueTupleFactory.ArgumentTypes, false);
            var mappedCall = FuncCallLog<TTarget, TReturn>.MapCalls<TNewArgs>(Calls, valueTupleFactory);
            
            return new FuncCallLog<TTarget, TReturn, TNewArgs>(mappedCall, ParsedCallExpression);
        }
        
        public int Count => Calls.Count;
        
        private new IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>> Calls => 
            (IReadOnlyCollection<IFuncRecordedCall<TTarget, TReturn, TArgs>>) base.Calls;
    }
}