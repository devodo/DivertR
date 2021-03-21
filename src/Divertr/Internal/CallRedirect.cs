using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CallRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<object[], object?> _redirectDelegate;
        private readonly IEnumerable<ICallConstraint> _callConstraints;
        public object? State { get; }

        public CallRedirect(Func<object[], object?> redirectDelegate, IEnumerable<ICallConstraint> callConstraints)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
            _callConstraints = callConstraints ?? throw new ArgumentNullException(nameof(callConstraints));
        }

        public object? Invoke(ICall call)
        {
            return _redirectDelegate.Invoke(call.Arguments);
        }

        public bool IsMatch(ICall call)
        {
            return _callConstraints.All(callConstraint => callConstraint.IsMatch(call));
        }
    }
}