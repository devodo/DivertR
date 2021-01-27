using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace Divertr
{
    internal class RedirectionContext<T> where T : class
    {
        private readonly AsyncLocal<List<int>> _indexStack = new AsyncLocal<List<int>>();
        public T Origin { get; }
        private readonly List<Redirection<T>> _redirections;

        public RedirectionContext(T origin, List<Redirection<T>> redirections)
        {
            _redirections = redirections;
            Origin = origin;
        }

        public bool MoveNext(IInvocation invocation, out T redirect)
        {
            var indexStack = _indexStack.Value;
            var i = indexStack?.LastOrDefault() ?? 0;

            if (i >= _redirections.Count)
            {
                redirect = null;
                return false;
            }

            bool matched = false;
            T matchedRedirect = null;
            for (; i < _redirections.Count; i++)
            {
                if (_redirections[i].IsMatch(invocation))
                {
                    matched = true;
                    matchedRedirect = _redirections[i].RedirectTarget;
                    break;
                }
            }

            _indexStack.Value = (indexStack ?? new List<int>()).Append(i + 1).ToList();

            if (!matched)
            {
                redirect = null;
                return false;
            }

            redirect = matchedRedirect;
            return true;
        }

        public void MoveBack()
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
