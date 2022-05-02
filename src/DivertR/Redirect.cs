using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Redirect<TTarget> : IRedirect<TTarget>
        where TTarget : class
    {
        public Redirect(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget> callConstraint, IRedirectOptions<TTarget>? redirectOptions = null)
        {
            if (callHandler == null) throw new ArgumentNullException(nameof(callHandler));
            if (callConstraint == null) throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions<TTarget>.Default;
            CallHandler = redirectOptions.CallHandlerDecorator?.Invoke(callHandler) ?? callHandler;
            CallConstraint = redirectOptions.CallConstraintDecorator?.Invoke(callConstraint) ?? callConstraint;
            
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }
        
        public Redirect(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget>? redirectOptions = null)
            : this(callHandler, TrueCallConstraint<TTarget>.Instance, redirectOptions)
        {
        }

        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallConstraint<TTarget> CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallHandler<TTarget> CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}