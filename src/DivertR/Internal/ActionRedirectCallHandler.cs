using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ActionRedirectCallHandler<TTarget> : CallHandler<TTarget> where TTarget : class?
    {
        private readonly Action<IActionRedirectCall<TTarget>> _handlerDelegate;

        public ActionRedirectCallHandler(Action<IActionRedirectCall<TTarget>> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new ActionRedirectCall<TTarget>(call);
            _handlerDelegate.Invoke(redirectCall);

            return default;
        }
    }
    
    internal class ActionCallHandler : ICallHandler
    {
        private readonly Action _handlerDelegate;

        public ActionCallHandler(Action handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            _handlerDelegate.Invoke();

            return default;
        }
    }

    internal class ActionRedirectCallHandler<TTarget, TArgs> : CallHandler<TTarget>
        where TTarget : class?
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Action<IActionRedirectCall<TTarget, TArgs>> _handlerDelegate;

        public ActionRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            Action<IActionRedirectCall<TTarget, TArgs>> handlerDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Handle(IRedirectCall<TTarget> call)
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
}