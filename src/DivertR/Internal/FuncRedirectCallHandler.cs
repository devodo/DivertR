using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, object?> _redirectDelegate;

        public FuncRedirectCallHandler(
            Func<IRedirectCall, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
    
    internal class FuncArgsRedirectCallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, CallArguments, object?> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            Func<IRedirectCall, CallArguments, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call, call.Args);
        }
    }
    
    internal class FuncRedirectCallHandler<TTarget, TReturn> : CallHandler<TTarget> where TTarget : class
    {
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> _redirectDelegate;

        public FuncRedirectCallHandler(
            Func<IFuncRedirectCall<TTarget, TReturn>, TReturn> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(IRedirectCall<TTarget> call)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(call);

            return _redirectDelegate.Invoke(redirectCall);
        }
    }

    internal class FuncRedirectCallHandler<TTarget, TReturn, TArgs> : CallHandler<TTarget>
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
        protected override object? Call(IRedirectCall<TTarget> call)
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
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn> : CallHandler<TTarget>
        where TTarget : class
    {
        private readonly Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> _redirectDelegate;

        public FuncArgsRedirectCallHandler(
            Func<IFuncRedirectCall<TTarget, TReturn>, CallArguments, TReturn> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(IRedirectCall<TTarget> call)
        {
            var redirectCall = new FuncRedirectCall<TTarget, TReturn>(call);

            return _redirectDelegate.Invoke(redirectCall, redirectCall.Args);
        }
    }
    
    internal class FuncArgsRedirectCallHandler<TTarget, TReturn, TArgs> : CallHandler<TTarget>
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
        protected override object? Call(IRedirectCall<TTarget> call)
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