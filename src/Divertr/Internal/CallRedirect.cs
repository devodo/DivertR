using System;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class CallRedirect<T> : IRedirect<T> where T : class
    {
        private readonly Func<object[], object?> _redirectDelegate;
        private readonly ICallCondition _callCondition;
        public object? State { get; }

        public CallRedirect(Func<object[], object?> redirectDelegate, ICallCondition callCondition)
        {
            _redirectDelegate = redirectDelegate;
            _callCondition = callCondition;
        }

        public object? Invoke(MethodInfo methodInfo, object[] args)
        {
            return _redirectDelegate.Invoke(args);
        }

        public bool IsMatch(ICall call)
        {
            return _callCondition.IsMatch(call);
        }
    }
}
