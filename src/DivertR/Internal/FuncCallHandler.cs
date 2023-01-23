using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncCallHandler<TTarget, TReturn> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, TReturn?> _handlerDelegate;

        public FuncCallHandler(
            Func<IFuncRedirectCall<TTarget, TReturn>, TReturn?> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(call);

            return _handlerDelegate.Invoke(redirectCall);
        }
    }

    internal class FuncCallHandler<TTarget, TReturn, TArgs> : ICallHandler<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn?> _handlerDelegate;

        public FuncCallHandler(
            IValueTupleMapper valueTupleMapper,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn?> handlerDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(call, valueTupleArgs);

            try
            {
                return _handlerDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }
        }
    }
    
    internal class FuncCallHandler<TReturn> : ICallHandler
    {
        private readonly Func<IFuncRedirectCall<TReturn>, TReturn?> _handlerDelegate;

        public FuncCallHandler(
            Func<IFuncRedirectCall<TReturn>, TReturn?> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var redirectCall = new FuncRedirectCall<TReturn>(call);

            return _handlerDelegate.Invoke(redirectCall);
        }
    }
}