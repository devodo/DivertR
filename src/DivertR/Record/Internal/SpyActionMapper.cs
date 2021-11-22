using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class SpyActionMapper<TTarget, TMap> where TTarget : class
    {
        private readonly Func<IActionRecordedCall<TTarget>, TMap> _mapper;

        public SpyActionMapper(Func<IActionRecordedCall<TTarget>, TMap> mapper)
        {
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var actionRecordedCall = new ActionRecordedCall<TTarget>(recordedCall);

            return _mapper.Invoke(actionRecordedCall);
        }
    }
    
    internal class SpyActionMapper<TTarget, TArgs, TMap>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IActionRecordedCall<TTarget, TArgs>, TMap> _mapper;

        public SpyActionMapper(IValueTupleMapper valueTupleMapper, Func<IActionRecordedCall<TTarget, TArgs>, TMap> mapper)
        {
            _valueTupleMapper = valueTupleMapper;
            _mapper = mapper;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMap Map(IRecordedCall<TTarget> recordedCall)
        {
            var args = (TArgs) _valueTupleMapper.ToTuple(recordedCall.Args.InternalArgs);
            var actionRecordedCall = new ActionRecordedCall<TTarget, TArgs>(recordedCall, args);

            return _mapper.Invoke(actionRecordedCall);
        }
    }
}