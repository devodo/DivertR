using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionRedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Action<IActionRedirectCall<TTarget>> _redirectDelegate;

        public ActionRedirectCallHandler(Action<IActionRedirectCall<TTarget>> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(call);
            _redirectDelegate.Invoke(redirectCall);

            return default;
        }
    }

    internal class ActionRedirectCallHandler<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>> _redirectDelegate;

        public ActionRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            Action<IActionRedirectCall<TTarget, TArgs>> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(call, valueTupleArgs);

            try
            {
                _redirectDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
    
    internal class ActionArgsRedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Action<IActionRedirectCall<TTarget>, CallArguments> _redirectDelegate;

        public ActionArgsRedirectCallHandler(Action<IActionRedirectCall<TTarget>, CallArguments> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(call);
            _redirectDelegate.Invoke(redirectCall, redirectCall.Args);

            return default;
        }
    }
    
    internal class ActionArgsRedirectCallHandler<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>, TArgs> _redirectDelegate;

        public ActionArgsRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            Action<IActionRedirectCall<TTarget, TArgs>, TArgs> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(call, valueTupleArgs);

            try
            {
                _redirectDelegate.Invoke(redirectCall, valueTupleArgs);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
}