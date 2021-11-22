using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCallHandler<TTarget, TReturn> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget, TReturn> _relay;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> _redirectDelegate;

        public FuncRedirectCallHandler(
            IRelay<TTarget, TReturn> relay,
            Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate)
        {
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(callInfo, _relay);

            return _redirectDelegate.Invoke(redirectCall);
        }
    }

    internal class FuncRedirectCallHandler<TTarget, TReturn, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly IRelay<TTarget, TReturn> _relay;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> _redirectDelegate;

        public FuncRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            IRelay<TTarget, TReturn> relay,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(callInfo.Arguments.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(callInfo, _relay, valueTupleArgs);

            try
            {
                return _redirectDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(callInfo.Arguments.InternalArgs, valueTupleArgs);
            }
        }
    }
}