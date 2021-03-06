using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class RedirectState<T> where T : class
    {
        private class InternalData
        {
            public T? Original { get; }
            public List<Redirect<T>> Redirects { get; }
            public InternalData(T? original, List<Redirect<T>> redirects)
            {
                Original = original;
                Redirects = redirects;
            }
        }
        
        private readonly InternalData _data;
        private readonly int _index;
        public T? Original => _data.Original;
        public ICall Call { get; }

        public static RedirectState<T>? Create(T? original, List<Redirect<T>> redirects, ICall call)
        {
            var index = GetNextIndex(-1, redirects, call);

            if (index == -1)
            {
                return null;
            }
            
            var data = new InternalData(original, redirects);
            return new RedirectState<T>(data, index, call);
        }

        private RedirectState(InternalData data, int index, ICall call)
        {
            Call = call;
            _data = data;
            _index = index;
        }

        public Redirect<T> Current => _data.Redirects[_index];

        public RedirectState<T>? MoveNext(ICall call)
        {
            var index = GetNextIndex(_index, _data.Redirects, call);

            if (index == -1)
            {
                return null;
            }

            return new RedirectState<T>(_data, index, call);
        }
        
        private static int GetNextIndex(int index, List<Redirect<T>> redirects, ICall call)
        {
            var startIndex = index + 1;

            for (var i = startIndex; i < redirects.Count; i++)
            {
                if (!redirects[i].IsMatch(call))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}