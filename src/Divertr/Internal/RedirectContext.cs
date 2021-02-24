using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RedirectContext<T> where T : class
    {
        private readonly AsyncLocal<List<int>> _indexStack = new AsyncLocal<List<int>>();
        public T? Original { get; }
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirects;

        public RedirectContext(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            _redirects = redirects;
            Original = original;
            RootInvocation = rootInvocation;
        }

        public Redirect<T>? Current
        {
            get
            {
                var index = _indexStack.Value?.LastOrDefault() ?? 0;

                return index > 0 
                    ? _redirects[index - 1]
                    : null;
            }
        }

        public Redirect<T>? BeginNextRedirect(IInvocation invocation)
        {
            var indexStack = _indexStack.Value ?? new List<int>();
            var i = indexStack.LastOrDefault();

            if (i >= _redirects.Count)
            {
                return null;
            }
            
            Redirect<T>? matchedRedirect = null;
            for (; i < _redirects.Count; i++)
            {
                if (_redirects[i].IsMatch(invocation))
                {
                    matchedRedirect = _redirects[i];
                    break;
                }
            }
            
            _indexStack.Value = indexStack.Append(i + 1).ToList();
            
            return matchedRedirect;
        }

        public void EndRedirect()
        {
            var indexStack = _indexStack.Value;

            if (indexStack == null || indexStack.Count == 0)
            {
                return;
            }
            
            indexStack.RemoveAt(indexStack.Count - 1);
        }
    }
}