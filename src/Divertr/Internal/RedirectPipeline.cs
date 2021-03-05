using System.Collections.Generic;
using System.Collections.Immutable;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class RedirectPipeline<T> where T : class
    {
        private class InternalData
        {
            public T? Original { get; }
            public List<Redirect<T>> Redirects { get; }

            public IInvocation RootInvocation { get; }

            public InternalData(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
            {
                Original = original;
                Redirects = redirects;
                RootInvocation = rootInvocation;
            }
        }
        
        private readonly ImmutableStack<int> _indexStack;
        private readonly InternalData _data;
        public T? Original => _data.Original;
        public IInvocation RootInvocation => _data.RootInvocation;

        public static RedirectPipeline<T>? Create(T? original, List<Redirect<T>> redirects, IInvocation rootInvocation)
        {
            var index = GetNextIndex(ImmutableStack<int>.Empty, redirects, rootInvocation);

            if (index == -1)
            {
                return null;
            }
            
            var data = new InternalData(original, redirects, rootInvocation);
            var indexStack = ImmutableStack<int>.Empty.Push(index);

            return new RedirectPipeline<T>(data, indexStack);
        }

        private RedirectPipeline(InternalData data, ImmutableStack<int> indexStack)
        {
            _data = data;
            _indexStack = indexStack;
        }

        public Redirect<T> Current
        {
            get
            {
                var index = _indexStack.Peek();
                return _data.Redirects[index];
            }
        }

        public RedirectPipeline<T>? BeginNextRedirect(IInvocation invocation)
        {
            var index = GetNextIndex(_indexStack, _data.Redirects, invocation);

            if (index == -1)
            {
                return null;
            }

            return new RedirectPipeline<T>(_data, _indexStack.Push(index));
        }

        public RedirectPipeline<T> EndRedirect(IInvocation invocation)
        {
            var indexStack = _indexStack.Pop();
            return new RedirectPipeline<T>(_data, indexStack);
        }

        private static int GetNextIndex(ImmutableStack<int> indexStack, List<Redirect<T>> redirects, IInvocation invocation)
        {
            var startIndex = indexStack.IsEmpty ? 0 : indexStack.Peek() + 1;

            for (var i = startIndex; i < redirects.Count; i++)
            {
                if (!redirects[i].IsMatch(invocation))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}