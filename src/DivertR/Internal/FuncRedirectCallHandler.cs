using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCallHandler<TTarget, TReturn> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> _redirectDelegate;

        public FuncRedirectCallHandler(
            Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(call);

            return _redirectDelegate.Invoke(redirectCall);
        }
    }

    internal class FuncRedirectCallHandler<TTarget, TReturn, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> _redirectDelegate;

        public FuncRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(call, valueTupleArgs);

            try
            {
                return _redirectDelegate.Invoke(redirectCall);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }
        }
    }
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn> : ICallHandler<TTarget>
        where TTarget : class
    {
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(call);

            return _redirectDelegate.Invoke(redirectCall, redirectCall.Args);
        }
    }
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleMapper _valueTupleMapper;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            IValueTupleMapper valueTupleMapper,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TArgs, TReturn> redirectDelegate)
        {
            _valueTupleMapper = valueTupleMapper;
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            var valueTupleArgs = (TArgs) _valueTupleMapper.ToTuple(call.Args.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(call, valueTupleArgs);

            try
            {
                return _redirectDelegate.Invoke(redirectCall, valueTupleArgs);
            }
            finally
            {
                _valueTupleMapper.WriteBackReferences(call.Args.InternalArgs, valueTupleArgs);
            }
        }
    }
}