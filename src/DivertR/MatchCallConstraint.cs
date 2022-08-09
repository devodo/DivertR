using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class MatchCallConstraint<TTarget> : ICallConstraint<TTarget> where TTarget : class?
    {
        private readonly Func<ICallInfo<TTarget>, bool> _matchFunc;

        public MatchCallConstraint(Func<ICallInfo<TTarget>, bool> matchFunc)
        {
            _matchFunc = matchFunc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICallInfo<TTarget> callInfo)
        {
            return _matchFunc.Invoke(callInfo);
        }
    }
}