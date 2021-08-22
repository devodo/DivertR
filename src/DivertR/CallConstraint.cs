using System;
using DivertR.Core;

namespace DivertR
{
    public class CallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, bool> _matchFunc;

        public CallConstraint(Func<CallInfo<TTarget>, bool> matchFunc)
        {
            _matchFunc = matchFunc;
        }
        
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _matchFunc.Invoke(callInfo);
        }
    }
}
