using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class DelegateRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<object[], object?> _redirectDelegate;
        private readonly ICallConstraint _callConstraint;
        public object? State { get; }

        public DelegateRedirect(Func<object[], object?> redirectDelegate, ICallConstraint callConstraint)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            _callConstraint = callConstraint ?? throw new ArgumentNullException(nameof(callConstraint));
        }

        public object? Invoke(ICall call)
        {
            return _redirectDelegate.Invoke(call.Arguments);
        }

        public bool IsMatch(ICall call)
        {
            return _callConstraint.IsMatch(call);
        }
    }
}