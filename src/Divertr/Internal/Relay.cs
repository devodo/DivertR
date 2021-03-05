using System.Collections.Immutable;
using System.Collections.Generic;
using System.Threading;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class Relay<T> : IRelay<T> where T : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectPipeline<T>>> _callStack = new AsyncLocal<ImmutableStack<RedirectPipeline<T>>>();
        public T Next { get; }
        
        public T Original { get; }
        
        public T? OriginalInstance => Current.Original;

        public object? State => Current.Current.State;

        public Relay()
        {
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(this);
            Original = ProxyFactory.Instance.CreateOriginalTargetProxy(this);
        }
        
        public Redirect<T>? BeginCall(T? original, List<Redirect<T>> redirects, IInvocation invocation)
        {
            var pipeline = RedirectPipeline<T>.Create(original, redirects, invocation);

            if (pipeline == null)
            {
                return null;
            }
            
            var callStack = _callStack.Value ?? ImmutableStack<RedirectPipeline<T>>.Empty;
            _callStack.Value = callStack.Push(pipeline);
            
            return pipeline.Current;
        }

        public void EndCall(IInvocation invocation)
        {
            var callStack = _callStack.Value;
            var redirectRelay = callStack.Peek();

            if (!ReferenceEquals(invocation, redirectRelay.RootInvocation))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect relay for the current call");
            }
            
            _callStack.Value = callStack.Pop();
        }

        public Redirect<T>? BeginNextRedirect(IInvocation invocation)
        {
            var callStack = _callStack.Value;
            var pipeline = callStack.Peek().BeginNextRedirect(invocation);

            if (pipeline == null)
            {
                return null;
            }
            
            _callStack.Value = callStack.Pop().Push(pipeline);

            return pipeline.Current;
        }

        public void EndRedirect(IInvocation invocation)
        {
            var pipeline = Current.EndRedirect(invocation);
            var callStack = _callStack.Value.Pop().Push(pipeline);
            _callStack.Value = callStack;
        }

        public RedirectPipeline<T> Current => _callStack.Value.Peek();
    }
}