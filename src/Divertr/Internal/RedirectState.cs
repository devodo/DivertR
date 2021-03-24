using System.Collections.Generic;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class RedirectState<T> where T : class
    {
        private readonly InternalData _data;
        private readonly int _index;
        
        public T Proxy => _data.Proxy;
        public T? Original => _data.Original;
        public CallInfo CallInfo { get; }
        public IRedirect<T> Redirect => _data.Redirects[_index];

        public static RedirectState<T>? Create(T proxy, T? original, List<IRedirect<T>> redirects, CallInfo callInfo)
        {
            var index = GetNextIndex(-1, redirects, callInfo);

            if (index == -1)
            {
                return null;
            }
            
            var data = new InternalData(proxy, original, redirects);
            return new RedirectState<T>(data, index, callInfo);
        }

        private RedirectState(InternalData data, int index, CallInfo callInfo)
        {
            CallInfo = callInfo;
            _data = data;
            _index = index;
        }

        public RedirectState<T>? MoveNext(CallInfo callInfo)
        {
            var index = GetNextIndex(_index, _data.Redirects, callInfo);

            if (index == -1)
            {
                return null;
            }

            return new RedirectState<T>(_data, index, callInfo);
        }
        
        private static int GetNextIndex(int index, List<IRedirect<T>> redirects, CallInfo callInfo)
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
        
        private class InternalData
        {
            public T Proxy { get; }
            public T? Original { get; }
            public List<IRedirect<T>> Redirects { get; }

            public InternalData(T proxy, T? original, List<IRedirect<T>> redirects)
            {
                Proxy = proxy;
                Original = original;
                Redirects = redirects;
            }
        }
    }
}