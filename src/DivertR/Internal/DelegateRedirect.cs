using System;
using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, object?> _redirectDelegate;

        public DelegateRedirect(Func<CallInfo<TTarget>, object?> redirectDelegate, ICallConstraint<TTarget> callConstraint)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            CallConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
        }

        public ICallConstraint<TTarget> CallConstraint { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _redirectDelegate.Invoke(callInfo);
        }
    }
}