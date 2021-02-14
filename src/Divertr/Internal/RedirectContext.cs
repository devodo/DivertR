﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class RedirectContext<T> where T : class
    {
        private readonly AsyncLocal<List<int>> _indexStack = new AsyncLocal<List<int>>();
        public T? Root { get; }
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirects;

        public RedirectContext(T? root, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            _redirects = redirects;
            Root = root;
            RootInvocation = rootInvocation;
        }

        public bool MoveNext(IInvocation invocation, out T? redirect)
        {
            var indexStack = _indexStack.Value ?? new List<int>();
            var i = indexStack.LastOrDefault();

            if (i >= _redirects.Count)
            {
                redirect = null;
                return false;
            }

            bool matched = false;
            T? matchedRedirect = null;
            for (; i < _redirects.Count; i++)
            {
                if (_redirects[i].IsMatch(invocation))
                {
                    matched = true;
                    matchedRedirect = _redirects[i].Target;
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