using System.Runtime.CompilerServices;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectIndex<TTarget> where TTarget : class
    {
        private readonly RedirectConfiguration<TTarget> _redirectConfiguration;
        private readonly int _index;

        public bool StrictSatisfied { get; }

        public CallInfo<TTarget> CallInfo { get; }
        public Redirect<TTarget> Redirect => _redirectConfiguration.Redirects[_index];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RedirectIndex<TTarget>? Create(RedirectConfiguration<TTarget> redirectConfiguration, CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirectConfiguration.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictSatisfied = !redirectConfiguration.IsStrictMode || redirectConfiguration.Redirects[index].ExcludeStrict;
            
            return new RedirectIndex<TTarget>(redirectConfiguration, index, callInfo, strictSatisfied);
        }

        private RedirectIndex(RedirectConfiguration<TTarget> redirectConfiguration, int index, CallInfo<TTarget> callInfo, bool strictSatisfied)
        {
            _redirectConfiguration = redirectConfiguration;
            CallInfo = callInfo;
            _index = index;
            StrictSatisfied = strictSatisfied;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RedirectIndex<TTarget>? MoveNext(CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirectConfiguration.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            var strictVisited = StrictSatisfied || _redirectConfiguration.Redirects[index].ExcludeStrict;

            return new RedirectIndex<TTarget>(_redirectConfiguration, index, callInfo, strictVisited);
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