using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RelayIndex<TTarget> where TTarget : class?
    {
        private readonly IRedirectPlan _redirectPlan;
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

        public IRedirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _redirectPlan.Stack[_index].Redirect;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RelayIndex<TTarget>? Create(IRedirectPlan redirectPlan, ICallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirectPlan.Stack, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = !redirectPlan.IsStrictMode || !redirectPlan.Stack[index].Options.DisableSatisfyStrict;
            
            return new RelayIndex<TTarget>(redirectPlan, index, callInfo, strictSatisfied);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RelayIndex(IRedirectPlan redirectPlan, int index, ICallInfo<TTarget> callInfo, bool strictSatisfied)
        {
            _redirectPlan = redirectPlan;
            CallInfo = callInfo;
            _index = index;
            StrictSatisfied = strictSatisfied;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RelayIndex<TTarget>? MoveNext(ICallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirectPlan.Stack, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = StrictSatisfied || !_redirectPlan.Stack[index].Options.DisableSatisfyStrict;

            return new RelayIndex<TTarget>(_redirectPlan, index, callInfo, strictSatisfied);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNextIndex(int index, IReadOnlyList<IRedirectContainer> stack, ICallInfo callInfo)
        {
            var startIndex = index + 1;

            for (var i = startIndex; i < stack.Count; i++)
            {
                if (!stack[i].Redirect.IsMatch(callInfo))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}