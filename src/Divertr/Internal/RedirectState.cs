using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectState<T> where T : class
    {
        private readonly int _index;

        private readonly List<IRedirect<T>> _redirects;
        public CallInfo<T> CallInfo { get; }
        public IRedirect<T> Redirect => _redirects[_index];

        public static RedirectState<T>? Create(List<IRedirect<T>> redirects, CallInfo<T> callInfo)
        {
            var index = GetNextIndex(-1, redirects, callInfo);

            if (index == -1)
            {
                return null;
            }
            
            return new RedirectState<T>(redirects, index, callInfo);
        }

        private RedirectState(List<IRedirect<T>> redirects, int index, CallInfo<T> callInfo)
        {
            CallInfo = callInfo;
            _redirects = redirects;
            _index = index;
        }

        public RedirectState<T>? MoveNext(CallInfo<T> callInfo)
        {
            var index = GetNextIndex(_index, _redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            return new RedirectState<T>(_redirects, index, callInfo);
        }
        
        private static int GetNextIndex(int index, List<IRedirect<T>> redirects, CallInfo<T> callInfo)
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