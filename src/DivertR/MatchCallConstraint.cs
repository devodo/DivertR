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
}
