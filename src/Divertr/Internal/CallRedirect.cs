using System;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CallRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<object[], object?> _redirectDelegate;
        private readonly ICallConstraint _callConstraint;
        public object? State { get; }

        public CallRedirect(Func<object[], object?> redirectDelegate, ICallConstraint? callCondition)
        {
            _redirectDelegate = redirectDelegate;
            _callConstraint = callCondition ?? TrueCallConstraint.Instance;
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