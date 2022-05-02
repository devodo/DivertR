using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class MatchCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, bool> _matchFunc;

        public MatchCallConstraint(Func<CallInfo<TTarget>, bool> matchFunc)
        {
            _matchFunc = matchFunc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(CallInfo<TTarget> callInfo)
        {
            return _matchFunc.Invoke(callInfo);
        }
    }
}