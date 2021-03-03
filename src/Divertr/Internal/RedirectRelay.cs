using System.Collections.Generic;
using System.Collections.Immutable;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class RedirectRelay<T> where T : class
    {
        private readonly ImmutableStack<int> _indexStack = ImmutableStack<int>.Empty;
        public T? Original { get; }
        public IInvocation RootInvocation { get; }
        private readonly List<Redirect<T>> _redirects;

        public RedirectRelay(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            _redirects = redirects;
            Original = original;
            RootInvocation = rootInvocation;
        }

        private RedirectRelay(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation, ImmutableStack<int> indexStack)
            : this(original, redirects, rootInvocation)
        {
            _indexStack = indexStack;
        }

        public Redirect<T> Current
        {
            get
            {
                var index = _indexStack.Peek();
                return _redirects[index];
            }
        }

        public RedirectRelay<T>? BeginNextRedirect(IInvocation invocation)
        {
            var startIndex = _indexStack.IsEmpty ? 0 : _indexStack.Peek() + 1;

            for (var i = startIndex; i < _redirects.Count; i++)
            {
                if (!_redirects[i].IsMatch(invocation))
                {
                    continue;
                }

                var indexStack = _indexStack.Push(i);

                return new RedirectRelay<T>(Original, _redirects, RootInvocation, indexStack);
            }

            return null;
        }

        public RedirectRelay<T> EndRedirect(IInvocation invocation)
        {
            var indexStack = _indexStack.Pop();
            return new RedirectRelay<T>(Original, _redirects, RootInvocation, indexStack);
        }
    }
}