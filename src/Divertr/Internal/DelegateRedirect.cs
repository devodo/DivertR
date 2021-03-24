using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<CallInfo, object?> _redirectDelegate;
        private readonly ICallConstraint _callConstraint;

        public DelegateRedirect(Func<CallInfo, object?> redirectDelegate, ICallConstraint callConstraint)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
        }

        public object? Call(CallInfo callInfo)
        {
            return _redirectDelegate.Invoke(callInfo);
        }

        public bool IsMatch(CallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}