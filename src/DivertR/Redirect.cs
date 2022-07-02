using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Redirect : IRedirect
    {
        private readonly ICallHandler _callHandler;
        private readonly ICallConstraint _callConstraint;

        public Redirect(ICallHandler callHandler, ICallConstraint callConstraint, IRedirectOptions? redirectOptions = null)
        {
            _callHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions.Default;
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }
        
        public Redirect(ICallHandler callHandler, IRedirectOptions? redirectOptions = null)
            : this(callHandler, TrueCallConstraint.Instance, redirectOptions)
        {
        }

        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
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
    
    public class Redirect<TTarget> : IRedirect where TTarget : class
    {
        private readonly ICallHandler<TTarget> _callHandler;
        private readonly ICallConstraint<TTarget> _callConstraint;

        public Redirect(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget> callConstraint, IRedirectOptions? redirectOptions = null)
        {
            _callHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions.Default;
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }
        
        public Redirect(ICallHandler<TTarget> callHandler, IRedirectOptions? redirectOptions = null)
            : this(callHandler, TrueCallConstraint<TTarget>.Instance, redirectOptions)
        {
        }

        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo callInfo)
        {
            if (!(callInfo is ICallInfo<TTarget> callOfTTarget))
            {
                throw new ArgumentException($"Redirect target type {typeof(TTarget)} invalid for ICallInfo type: {callInfo.GetType()}", nameof(callInfo));
            }
            
            return _callConstraint.IsMatch(callOfTTarget);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            if (!(call is IRedirectCall<TTarget> callOfTTarget))
            {
                throw new ArgumentException($"Redirect target type {typeof(TTarget)} invalid for IRedirectCall type: {call.GetType()}", nameof(call));
            }
            
            return _callHandler.Handle(callOfTTarget);
        }
    }
}