using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectContext<TTarget> where TTarget : class
    {
        private readonly int _index;

        private readonly IList<IRedirect<TTarget>> _redirects;
        public CallInfo<TTarget> CallInfo { get; }
        public IRedirect<TTarget> Redirect => _redirects[_index];

        public static RedirectContext<TTarget>? Create(IList<IRedirect<TTarget>> redirects, CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(-1, redirects, callInfo);

            if (index == -1)
            {
                return null;
            }
            
            return new RedirectContext<TTarget>(redirects, index, callInfo);
        }

        private RedirectContext(IList<IRedirect<TTarget>> redirects, int index, CallInfo<TTarget> callInfo)
        {
            CallInfo = callInfo;
            _redirects = redirects;
            _index = index;
        }

        public RedirectContext<TTarget>? MoveNext(CallInfo<TTarget> callInfo)
        {
            var index = GetNextIndex(_index, _redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            return new RedirectContext<TTarget>(_redirects, index, callInfo);
        }
        
        private static int GetNextIndex(int index, IList<IRedirect<TTarget>> redirects, CallInfo<TTarget> callInfo)
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