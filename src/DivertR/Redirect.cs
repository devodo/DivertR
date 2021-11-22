using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class Redirect<TTarget> where TTarget : class
    {
        public Redirect(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget>? callConstraint = null, int? orderWeight = null, bool? disableSatisfyStrict = null)
        {
            CallHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
            OrderWeight = orderWeight ?? 0;
            DisableSatisfyStrict = disableSatisfyStrict ?? false;
        }

        public ICallHandler<TTarget> CallHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public ICallConstraint<TTarget> CallConstraint
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