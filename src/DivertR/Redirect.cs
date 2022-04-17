using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public abstract class AbstractRedirect<TCallInfo, TRedirectCall> : IRedirect<TCallInfo, TRedirectCall>
        where TCallInfo : CallInfo
        where TRedirectCall : IRedirectCall
    {
        private readonly IBaseCallConstraint<TCallInfo> _callConstraint;
        private readonly IBaseCallHandler<TRedirectCall> _callHandler;
        
        protected AbstractRedirect(IBaseCallConstraint<TCallInfo> callConstraint, IBaseCallHandler<TRedirectCall> callHandler, IBaseRedirectOptions)
        {
            OrderWeight = orderWeight;
            DisableSatisfyStrict = disableSatisfyStrict;
            _callConstraint = callConstraint;
            _callHandler = callHandler;
        }
        
        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(TCallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(TRedirectCall call)
        {
            return _callHandler.Handle(call);
        }
    }
    
    public class Redirect : AbstractRedirect<CallInfo, IRedirectCall>
    {
        private readonly ICallConstraint _callConstraint;
        private readonly ICallHandler _callHandler;
        
        public Redirect(ICallHandler callHandler, ICallConstraint callConstraint, RedirectOptions? redirectOptions = null)
        {
            if (callHandler == null) throw new ArgumentNullException(nameof(callHandler));
            if (callConstraint == null) throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions.Default;
            _callHandler = redirectOptions.CallHandlerDecorator?.Invoke(callHandler) ?? callHandler;
            _callConstraint = redirectOptions.CallConstraintDecorator?.Invoke(callConstraint) ?? callConstraint;
            
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }

        public Redirect(ICallHandler callHandler, RedirectOptions? redirectOptions = null)
            : this(callHandler, TrueCallConstraint.Instance, redirectOptions)
        {
        }
    }

    public class Redirect<TTarget> : AbstractRedirect<CallInfo<TTarget>, IRedirectCall<TTarget>>
        where TTarget : class
    {
        private readonly ICallConstraint<TTarget> _callConstraint;
        private readonly ICallHandler<TTarget> _callHandler;
        
        public Redirect(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget> callConstraint, RedirectOptions<TTarget>? redirectOptions = null)
        {
            if (callHandler == null) throw new ArgumentNullException(nameof(callHandler));
            if (callConstraint == null) throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions<TTarget>.Default;
            _callHandler = redirectOptions.CallHandlerDecorator?.Invoke(callHandler) ?? callHandler;
            _callConstraint = redirectOptions.CallConstraintDecorator?.Invoke(callConstraint) ?? callConstraint;
            
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }
        
        public Redirect(ICallHandler<TTarget> callHandler, RedirectOptions<TTarget>? redirectOptions = null)
            : this(callHandler, TrueCallConstraint<TTarget>.Instance, redirectOptions)
        {
        }
    }
}