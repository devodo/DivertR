using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly Action<IActionRedirectCall<TTarget>> _handlerDelegate;

        public ActionCallHandler(Action<IActionRedirectCall<TTarget>> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(call);
            _handlerDelegate.Invoke(redirectCall);

            return default;
        }
    }

    internal class ActionCallHandler<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>> _handlerDelegate;

        public ActionCallHandler(
            IValueTupleMapper valueTupleMapper,
            Action<IActionRedirectCall<TTarget, TArgs>> handlerDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(call, valueTupleArgs);

            try
            {
                _handlerDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
    
    internal class ActionCallHandlerArgs<TTarget> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly Action<IActionRedirectCall<TTarget>, CallArguments> _handlerDelegate;

        public ActionCallHandlerArgs(Action<IActionRedirectCall<TTarget>, CallArguments> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(call);
            _handlerDelegate.Invoke(redirectCall, redirectCall.Args);

            return default;
        }
    }
    
    internal class ActionCallHandlerArgs<TTarget, TArgs> : ICallHandler<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>, TArgs> _handlerDelegate;

        public ActionCallHandlerArgs(
            IValueTupleMapper valueTupleMapper,
            Action<IActionRedirectCall<TTarget, TArgs>, TArgs> handlerDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new ActionRedirectCall<TTarget, TArgs>(call, valueTupleArgs);

            try
            {
                _handlerDelegate.Invoke(redirectCall, valueTupleArgs);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }

            return default;
        }
    }
}