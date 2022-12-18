using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Via : IVia
    {
        private readonly ICallHandler _callHandler;
        private readonly ICallConstraint _callConstraint;

        public Via(ICallHandler callHandler, ICallConstraint? callConstraint = null)
        {
            _callHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            _callConstraint = callConstraint ?? TrueCallConstraint.Instance;
        }
        
        public Via(ICallHandler callHandler)
            : this(callHandler, TrueCallConstraint.Instance)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _callHandler.Handle(call);
        }
    }
    
    public class Via<TTarget> : IVia where TTarget : class?
    {
        private readonly ICallHandler<TTarget> _callHandler;
        private readonly ICallConstraint<TTarget> _callConstraint;

        public Via(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget>? callConstraint = null)
        {
            _callHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            _callConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
        }
        
        public Via(ICallHandler<TTarget> callHandler)
            : this(callHandler, TrueCallConstraint<TTarget>.Instance)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            if (callInfo is not ICallInfo<TTarget> callOfTTarget)
            {
                throw new ArgumentException($"Via target type {typeof(TTarget)} invalid for ICallInfo type: {callInfo.GetType()}", nameof(callInfo));
            }
            
            return _callConstraint.IsMatch(callOfTTarget);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            if (call is not IRedirectCall<TTarget> callOfTTarget)
            {
                throw new ArgumentException($"Via target type {typeof(TTarget)} invalid for IRedirectCall type: {call.GetType()}", nameof(call));
            }
            
            return _callHandler.Handle(callOfTTarget);
        }
    }
}