using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionRedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;
        private readonly Action<IActionRedirectCall<TTarget>> _redirectDelegate;

        public ActionRedirectCallHandler(
            IRelay<TTarget> relay,
            Action<IActionRedirectCall<TTarget>> redirectDelegate)
        {
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(callInfo, _relay);
            _redirectDelegate.Invoke(redirectCall);

            return default;
        }
    }

    internal class ActionRedirectCallHandler<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly IRelay<TTarget> _relay;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>> _redirectDelegate;

        public ActionRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            IRelay<TTarget> relay,
            Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(callInfo.Arguments.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(callInfo, _relay, valueTupleArgs);

            try
            {
                _redirectDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(callInfo.Arguments.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
    
    internal class ActionArgsRedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;
        private readonly Action<IActionRedirectCall<TTarget>, CallArguments> _redirectDelegate;

        public ActionArgsRedirectCallHandler(
            IRelay<TTarget> relay,
            Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate)
        {
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(callInfo, _relay);
            _redirectDelegate.Invoke(redirectCall, redirectCall.Args);

            return default;
        }
    }
    
    internal class ActionArgsRedirectCallHandler<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly IRelay<TTarget> _relay;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>, TArgs> _redirectDelegate;

        public ActionArgsRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            IRelay<TTarget> relay,
            Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _relay = relay;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(callInfo.Arguments.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(callInfo, _relay, valueTupleArgs);

            try
            {
                _redirectDelegate.Invoke(redirectCall, valueTupleArgs);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(callInfo.Arguments.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
}