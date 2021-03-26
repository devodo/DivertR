using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<CallInfo<T>, object?> _redirectDelegate;
        private readonly ICallConstraint<T> _callConstraint;

        public DelegateRedirect(Func<CallInfo<T>, object?> redirectDelegate, ICallConstraint<T> callConstraint)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
        }

        public object? Call(CallInfo<T> callInfo)
        {
            return _redirectDelegate.Invoke(callInfo);
        }

        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}