using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace DivertR.Internal
{
    internal class CallRelay<T> : ICallRelay<T> where T : class
    {
        private readonly AsyncLocal<ImmutableStack<RedirectRelay<T>>> _callStack = new AsyncLocal<ImmutableStack<RedirectRelay<T>>>();
        public T Next { get; }
        
        public T Original { get; }
        
        public T? OriginalInstance => Current.Original;

        public object? State => Current.Current.State;

        public CallRelay()
        {
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(this);
            Original = ProxyFactory.Instance.CreateOriginalTargetProxy(this);
        }
        
        public Redirect<T>? BeginCall(T? original, List<Redirect<T>> redirects, IInvocation invocation)
        {
            var redirectRelay = new RedirectRelay<T>(original, redirects, invocation);
            redirectRelay = redirectRelay.BeginNextRedirect(invocation);

            if (redirectRelay == null)
            {
                return null;
            }

            var callStack = _callStack.Value ?? ImmutableStack<RedirectRelay<T>>.Empty;
            _callStack.Value = callStack.Push(redirectRelay);
            
            return redirectRelay.Current;
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

        public RedirectRelay<T>? BeginNextRedirect(IInvocation invocation)
        {
            var redirectRelay = Current.BeginNextRedirect(invocation);

            if (redirectRelay == null)
            {
                return null;
            }

            var callStack = _callStack.Value.Pop().Push(redirectRelay);
            _callStack.Value = callStack;

            return redirectRelay;
        }

        public void EndRedirect(IInvocation invocation)
        {
            var redirectRelay = Current.EndRedirect(invocation);
            var callStack = _callStack.Value.Pop().Push(redirectRelay);
            _callStack.Value = callStack;
        }

        public RedirectRelay<T> Current => _callStack.Value.Peek();
    }
}