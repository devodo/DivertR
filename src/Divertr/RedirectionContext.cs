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
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirections;

        public RedirectionContext(T origin, List<Redirect<T>> redirections, IInvocation rootInvocation)
        {
            _redirections = redirections;
            Origin = origin;
            RootInvocation = rootInvocation;
        }

        public bool MoveNext(IInvocation invocation, out T redirect)
        {
            var indexStack = _indexStack.Value ?? new List<int>();
            var i = indexStack.LastOrDefault();

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
                    matchedRedirect = _redirections[i].Target;
                    break;
                }
            }
            
            _indexStack.Value = indexStack.Append(i + 1).ToList();

            redirect = matchedRedirect;
            return matched;
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
