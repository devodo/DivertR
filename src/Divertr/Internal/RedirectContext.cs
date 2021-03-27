using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectContext<T> where T : class
    {
        private readonly int _index;

        private readonly IList<IRedirect<T>> _redirects;
        public CallInfo<T> CallInfo { get; }
        public IRedirect<T> Redirect => _redirects[_index];

        public static RedirectContext<T>? Create(IList<IRedirect<T>> redirects, CallInfo<T> callInfo)
        {
            var index = GetNextIndex(-1, redirects, callInfo);

            if (index == -1)
            {
                return null;
            }
            
            return new RedirectContext<T>(redirects, index, callInfo);
        }

        private RedirectContext(IList<IRedirect<T>> redirects, int index, CallInfo<T> callInfo)
        {
            CallInfo = callInfo;
            _redirects = redirects;
            _index = index;
        }

        public RedirectContext<T>? MoveNext(CallInfo<T> callInfo)
        {
            var index = GetNextIndex(_index, _redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            return new RedirectContext<T>(_redirects, index, callInfo);
        }
        
        private static int GetNextIndex(int index, IList<IRedirect<T>> redirects, CallInfo<T> callInfo)
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