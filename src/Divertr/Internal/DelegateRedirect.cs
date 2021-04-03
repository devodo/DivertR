using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirect<TTarget> : IRedirect<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, object?> _redirectDelegate;
        private readonly ICallConstraint<TTarget> _callConstraint;

        public DelegateRedirect(Func<CallInfo<TTarget>, object?> redirectDelegate, ICallConstraint<TTarget> callConstraint)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
        }

        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _redirectDelegate.Invoke(callInfo);
        }

        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}