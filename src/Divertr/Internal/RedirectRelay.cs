using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RedirectRelay<T> where T : class
    {
        private readonly AsyncLocal<List<int>> _indexStack = new AsyncLocal<List<int>>();
        public T? Original { get; }
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirects;

        public RedirectRelay(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            _redirects = redirects;
            Original = original;
            RootInvocation = rootInvocation;
        }

        public Redirect<T> Current
        {
            get
            {
                var indexStack = _indexStack.Value;

                if (indexStack == null || indexStack.Count == 0)
                {
                    throw new DiverterException("Members of this instance may only be accessed from within the context a Redirect call");
                }

                var index = indexStack[indexStack.Count - 1];

                return _redirects[index];
            }
        }

        public Redirect<T>? BeginNextRedirect(IInvocation invocation)
        {
            var indexStack = _indexStack.Value ?? RedirectRelay.EmptyList;
            var startIndex = indexStack.Count == 0 ? 0 : indexStack[indexStack.Count - 1] + 1;

            for (var i = startIndex; i < _redirects.Count; i++)
            {
                if (!_redirects[i].IsMatch(invocation))
                {
                    continue;
                }
                
                _indexStack.Value = indexStack.Append(i).ToList();
                return _redirects[i];
            }

            return null;
        }

        public void EndRedirect()
        {
            var indexStack = _indexStack.Value;

            if (indexStack == null || indexStack.Count == 0)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected end redirect state");
            }
            
            indexStack.RemoveAt(indexStack.Count - 1);
        }
    }
    
    internal static class RedirectRelay
    {
        public static readonly IReadOnlyList<int> EmptyList = new List<int>();
    }
}