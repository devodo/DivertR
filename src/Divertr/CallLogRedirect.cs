using System;
using DivertR.Core;
using DivertR.Internal;

namespace DivertR
{
    public class CallLogRedirect<T> : IRedirect<T> where T : class
    {
        private readonly IRelay<T> _relay;
        private readonly ICallConstraint<T> _callConstraint;

        public CallLogRedirect(IRelay<T> relay, ICallConstraint<T>? callConstraint = null)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _callConstraint = callConstraint ?? TrueCallConstraint<T>.Instance;
        }
        
        public object? Call(CallInfo<T> callInfo)
        {
            return _relay.CallNext(callInfo);
        }

        public bool IsMatch(CallInfo<T> callInfo)
        {
            return _callConstraint.IsMatch(callInfo);
        }
    }
}