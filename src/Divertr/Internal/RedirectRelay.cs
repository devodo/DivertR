using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class RedirectRelay<T> where T : class
    {
        private readonly List<int> _indexStack = new List<int>();
        public T? Original { get; }
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirects;

        public RedirectRelay(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            _redirects = redirects;
            Original = original;
            RootInvocation = rootInvocation;
        }

        private RedirectRelay(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation, List<int> indexStack)
            : this(original, redirects, rootInvocation)
        {
            _indexStack = indexStack;
        }

        public Redirect<T> Current
        {
            get
            {
                if (_indexStack.Count == 0)
                {
                    throw new DiverterException("Members of this instance may only be accessed from within the context a Redirect call");
                }

                var index = _indexStack[_indexStack.Count - 1];

                return _redirects[index];
            }
        }

        public RedirectRelay<T>? BeginNextRedirect(IInvocation invocation)
        {
            var startIndex = _indexStack.Count == 0 ? 0 : _indexStack[_indexStack.Count - 1] + 1;

            for (var i = startIndex; i < _redirects.Count; i++)
            {
                if (!_redirects[i].IsMatch(invocation))
                {
                    continue;
                }

                var indexStack = _indexStack.ToList();
                indexStack.Add(i);
                
                return new RedirectRelay<T>(Original, _redirects, RootInvocation, indexStack);
            }

            return null;
        }

        public RedirectRelay<T> EndRedirect(IInvocation invocation)
        {
            if (_indexStack.Count == 0)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected end redirect state");
            }
            
            var indexStack = _indexStack.ToList();
            indexStack.RemoveAt(indexStack.Count - 1);
            
            return new RedirectRelay<T>(Original, _redirects, RootInvocation, indexStack);
        }
    }
}