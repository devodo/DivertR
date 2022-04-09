using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Redirect
    {
        public Redirect(ICallHandler callHandler, ICallConstraint callConstraint, RedirectOptions? redirectOptions = null)
        {
            if (callHandler == null) throw new ArgumentNullException(nameof(callHandler));
            if (callConstraint == null) throw new ArgumentNullException(nameof(callConstraint));
            
            redirectOptions ??= RedirectOptions.Default;
            CallHandler = redirectOptions.CallHandlerDecorator?.Invoke(callHandler) ?? callHandler;
            CallConstraint = redirectOptions.CallConstraintDecorator?.Invoke(callConstraint) ?? callConstraint;
            
            OrderWeight = redirectOptions.OrderWeight ?? 0;
            DisableSatisfyStrict = redirectOptions.DisableSatisfyStrict ?? false;
        }
        
        public Redirect(ICallHandler callHandler, RedirectOptions? redirectOptions = null)
            : this(callHandler, TrueCallConstraint.Instance, redirectOptions)
        {
        }

        public ICallHandler CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallConstraint CallConstraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public int OrderWeight { get; }

        public bool DisableSatisfyStrict
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}