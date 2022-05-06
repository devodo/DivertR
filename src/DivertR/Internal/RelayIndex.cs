using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RelayIndex<TTarget> where TTarget : class
    {
        private readonly RedirectPlan<TTarget> _redirectPlan;
        private readonly int _index;

        public bool StrictSatisfied
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
        public ICallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public IRedirect<TTarget> Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan.Redirects[_index];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RelayIndex<TTarget>? Create(RedirectPlan<TTarget> redirectPlan, ICallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = !redirectPlan.IsStrictMode || !redirectPlan.Redirects[index].DisableSatisfyStrict;
            
            return new RelayIndex<TTarget>(redirectPlan, index, callInfo, strictSatisfied);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RelayIndex(RedirectPlan<TTarget> redirectPlan, int index, ICallInfo<TTarget> callInfo, bool strictSatisfied)
        {
            _redirectPlan = redirectPlan;
            CallInfo = callInfo;
            _index = index;
            StrictSatisfied = strictSatisfied;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RelayIndex<TTarget>? MoveNext(ICallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = StrictSatisfied || !_redirectPlan.Redirects[index].DisableSatisfyStrict;

            return new RelayIndex<TTarget>(_redirectPlan, index, callInfo, strictSatisfied);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNextIndex(int index, IReadOnlyList<IRedirect<TTarget>> redirects, ICallInfo<TTarget> callInfo)
        {
            var startIndex = index + 1;

            for (var i = startIndex; i < redirects.Count; i++)
            {
                if (!redirects[i].CallConstraint.IsMatch(callInfo))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}