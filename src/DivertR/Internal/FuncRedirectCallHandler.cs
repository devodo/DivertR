using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncRedirectCallHandler<TTarget, TReturn, TArgs> : ICallHandler<TTarget>
        where TTarget : class
        where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
    {
        private readonly IValueTupleFactory _valueTupleFactory;
        private readonly ReferenceArgumentMapper? _refMapper;
        private readonly IRelay<TTarget, TReturn> _relay;
        private readonly Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> _redirectDelegate;
        private readonly Func<CallInfo<TTarget>, object?> _callHandler;
        
        public FuncRedirectCallHandler(
            IValueTupleFactory valueTupleFactory,
            IRelay<TTarget, TReturn> relay,
            Func<IFuncRedirectCall<TTarget, TReturn, TArgs>, TReturn> redirectDelegate)
        {
            _valueTupleFactory = valueTupleFactory;
            _refMapper = _valueTupleFactory.GetRefMapper();
            _relay = relay;
            _redirectDelegate = redirectDelegate;

            if (_refMapper == null)
            {
                _callHandler = CallHandler;
            }
            else
            {
                _callHandler = CallHandlerWithRef;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _callHandler.Invoke(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? CallHandler(CallInfo<TTarget> callInfo)
        {
            var args = (TArgs) _valueTupleFactory.Create(callInfo.Arguments.InternalArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(callInfo, _relay, args);

            return _redirectDelegate.Invoke(redirectCall);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object? CallHandlerWithRef(CallInfo<TTarget> callInfo)
        {
            var mappedArgs = _refMapper!.MapToReferences(callInfo.Arguments.InternalArgs);
            var args = (TArgs) _valueTupleFactory.Create(mappedArgs);
            var redirectCall = new FuncRedirectCall<TTarget, TReturn, TArgs>(callInfo, _relay, args);
            
            try
            {
                return _redirectDelegate.Invoke(redirectCall);
            }
            finally
            {
                _refMapper.WriteReferences(mappedArgs, callInfo.Arguments.InternalArgs);
            }
        }
    }
}