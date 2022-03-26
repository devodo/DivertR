using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCallHandler<TTarget, TReturn> : CallHandler<TTarget> where TTarget : class
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
        protected override object? Call(CallInfo<TTarget> callInfo)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(callInfo, _relay);

            return _redirectDelegate.Invoke(redirectCall);
        }
    }

    internal class FuncRedirectCallHandler<TTarget, TReturn, TArgs> : CallHandler<TTarget>
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
        protected override object? Call(CallInfo<TTarget> callInfo)
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
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn> : CallHandler<TTarget>
        where TTarget : class
    {
        private readonly IRelay<TTarget, TReturn> _relay;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            IRelay<TTarget, TReturn> relay,
            Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate)
        {
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(CallInfo<TTarget> callInfo)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(callInfo, _relay);

            return _redirectDelegate.Invoke(redirectCall, redirectCall.Args);
        }
    }
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn, TArgs> : CallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly IRelay<TTarget, TReturn> _relay;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            IRelay<TTarget, TReturn> relay,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(CallInfo<TTarget> callInfo)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(callInfo.Arguments.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(callInfo, _relay, valueTupleArgs);

            try
            {
                return _redirectDelegate.Invoke(redirectCall, valueTupleArgs);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(callInfo.Arguments.InternalArgs, valueTupleArgs);
            }
        }
    }
}