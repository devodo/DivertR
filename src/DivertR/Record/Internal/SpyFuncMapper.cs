using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class SpyFuncMapper<TTarget, TReturn, TMap> where TTarget : class
    {
        private readonly Func<IFuncRecordedCall<TTarget, TReturn>, TMap> _mapper;

        public SpyFuncMapper(Func<IFuncRecordedCall<TTarget, TReturn>, TMap> mapper)
        {
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn>(recordedCall);

            return _mapper.Invoke(funcRecordedCall);
        }
    }
    
    internal class SpyFuncMapper<TTarget, TReturn, TArgs, TMap>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> _mapper;

        public SpyFuncMapper(IValueTupleMapper valueTupleMapper, Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TMap> mapper)
        {
            _valueTupleMapper = valueTupleMapper;
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var args = (TArgs) _valueTupleMapper.ToTuple(recordedCall.Args.InternalArgs);
            var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn, TArgs>(recordedCall, args);

            return _mapper.Invoke(funcRecordedCall);
        }
    }
    
    internal class SpyArgsFuncMapper<TTarget, TReturn, TMap>
        where TTarget : class
    {
        private readonly Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> _mapper;

        public SpyArgsFuncMapper(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper)
        {
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn>(recordedCall);

            return _mapper.Invoke(funcRecordedCall, funcRecordedCall.Args);
        }
    }
    
    internal class SpyArgsFuncMapper<TTarget, TReturn, TArgs, TMap>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> _mapper;

        public SpyArgsFuncMapper(IValueTupleMapper valueTupleMapper, Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper)
        {
            _valueTupleMapper = valueTupleMapper;
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var args = (TArgs) _valueTupleMapper.ToTuple(recordedCall.Args.InternalArgs);
            var funcRecordedCall = new FuncRecordedCall<TTarget, TReturn, TArgs>(recordedCall, args);

            return _mapper.Invoke(funcRecordedCall, args);
        }
    }
}