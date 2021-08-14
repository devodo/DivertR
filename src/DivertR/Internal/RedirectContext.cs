using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectContext<TTarget> where TTarget : class
    {
        private readonly int _index;
        private readonly RedirectState<TTarget> _redirectState;
        public CallInfo<TTarget> CallInfo { get; }
        public Redirect<TTarget> Redirect => _redirectState.RedirectItems[_index];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RedirectContext<TTarget>? Create(RedirectState<TTarget> redirectState, CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirectState.RedirectItems, callInfo);

            if (index == -1)
            {
                return null;
            }
            
            return new RedirectContext<TTarget>(redirectState, index, callInfo);
        }

        private RedirectContext(RedirectState<TTarget> redirectState, int index, CallInfo<TTarget> callInfo)
        {
            _redirectState = redirectState;
            CallInfo = callInfo;
            _index = index;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RedirectContext<TTarget>? MoveNext(CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirectState.RedirectItems, callInfo);

            if (index == -1)
            {
                return null;
            }

            return new RedirectContext<TTarget>(_redirectState, index, callInfo);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNextIndex(int index, Redirect<TTarget>[] redirectItems, CallInfo<TTarget> callInfo)
        {
            var startIndex = index + 1;

            for (var i = startIndex; i < redirectItems.Length; i++)
            {
                if (!redirectItems[i].CallConstraint.IsMatch(callInfo))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}