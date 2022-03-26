using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RelayStep<TTarget> where TTarget : class
    {
        private readonly RedirectPlan _redirectPlan;
        private readonly int _index;

        public bool StrictSatisfied
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public CallInfo<TTarget> CallInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public Redirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan.Redirects[_index];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RelayStep<TTarget>? Create(RedirectPlan redirectPlan, CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = !redirectPlan.IsStrictMode || !redirectPlan.Redirects[index].DisableSatisfyStrict;
            
            return new RelayStep<TTarget>(redirectPlan, index, callInfo, strictSatisfied);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RelayStep(RedirectPlan redirectPlan, int index, CallInfo<TTarget> callInfo, bool strictSatisfied)
        {
            _redirectPlan = redirectPlan;
            CallInfo = callInfo;
            _index = index;
            StrictSatisfied = strictSatisfied;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RelayStep<TTarget>? MoveNext(CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirectPlan.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = StrictSatisfied || !_redirectPlan.Redirects[index].DisableSatisfyStrict;

            return new RelayStep<TTarget>(_redirectPlan, index, callInfo, strictSatisfied);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNextIndex(int index, IReadOnlyList<Redirect> redirects, CallInfo callInfo)
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