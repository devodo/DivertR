using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RelayIndex<TCallInfo, TRedirect>
        where TCallInfo : CallInfo
        where TRedirect : IRedirect<TCallInfo>
    {
        private readonly RedirectPlan<TRedirect> _redirectPlan;
        private readonly int _index;

        public bool StrictSatisfied
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public TCallInfo CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public TRedirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan.Redirects[_index];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RelayIndex<TCallInfo, TRedirect>? Create(RedirectPlan<TRedirect> redirectPlan, TCallInfo callInfo)
        {
            var index = GetNextIndex(-1, redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = !redirectPlan.IsStrictMode || !redirectPlan.Redirects[index].DisableSatisfyStrict;
            
            return new RelayIndex<TCallInfo, TRedirect>(redirectPlan, index, callInfo, strictSatisfied);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RelayIndex(RedirectPlan<TRedirect> redirectPlan, int index, TCallInfo callInfo, bool strictSatisfied)
        {
            _redirectPlan = redirectPlan;
            CallInfo = callInfo;
            _index = index;
            StrictSatisfied = strictSatisfied;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RelayIndex<TCallInfo, TRedirect>? MoveNext(TCallInfo callInfo)
        {
            var index = GetNextIndex(_index, _redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = StrictSatisfied || !_redirectPlan.Redirects[index].DisableSatisfyStrict;

            return new RelayIndex<TCallInfo, TRedirect>(_redirectPlan, index, callInfo, strictSatisfied);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNextIndex(int index, IReadOnlyList<TRedirect> redirects, TCallInfo callInfo)
        {
            var startIndex = index + 1;

            for (var i = startIndex; i < redirects.Count; i++)
            {
                if (!redirects[i].IsMatch(callInfo))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}