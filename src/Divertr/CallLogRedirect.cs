using System;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class CallLogRedirect<T> : IRedirect<T> where T : class
    {
        private readonly IRelay<T> _relay;
        private readonly ICallConstraint _callConstraint;

        public CallLogRedirect(IRelay<T> relay, ICallConstraint? callConstraint = null)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _callConstraint = callConstraint ?? TrueCallConstraint.Instance;
        }
        
        public object? Call(CallInfo callInfo)
        {
            return _relay.CallNext(callInfo);
        }

        public bool IsMatch(CallInfo callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}