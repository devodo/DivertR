using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class MatchCallConstraint : ICallConstraint
    {
        private readonly Func<CallInfo, bool> _matchFunc;

        public MatchCallConstraint(Func<CallInfo, bool> matchFunc)
        {
            _matchFunc = matchFunc;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo callInfo)
        {
            return _matchFunc.Invoke(callInfo);
        }
    }
    
    public class MatchCallConstraint<TTarget> : CallConstraint<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, bool> _matchFunc;

        public MatchCallConstraint(Func<CallInfo<TTarget>, bool> matchFunc)
        {
            _matchFunc = matchFunc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _matchFunc.Invoke(callInfo);
        }
    }
}