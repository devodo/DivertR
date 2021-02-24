using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.DynamicProxy;

namespace Divertr.Internal
{
    internal class CallRelay<T> : ICallRelay<T> where T : class
    {
        private readonly AsyncLocal<List<RedirectRelay<T>>> _callStack = new AsyncLocal<List<RedirectRelay<T>>>();
        public T Next { get; }
        
        public T Original { get; }

        public object? State => Current.Current.State;

        public CallRelay()
        {
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(this);
            Original = ProxyFactory.Instance.CreateOriginalTargetProxy(this);
        }
        
        public Redirect<T>? BeginCall(T? original, List<Redirect<T>> redirects, IInvocation invocation)
        {
            var redirectRelay = new RedirectRelay<T>(original, redirects, invocation);
            var redirect = redirectRelay.BeginNextRedirect(invocation);

            if (redirect == null)
            {
                return null;
            }
            
            var callStack = _callStack.Value?.ToList() ?? new List<RedirectRelay<T>>();
            callStack.Add(redirectRelay);
            _callStack.Value = callStack;

            return redirect;
        }

        public void EndCall(IInvocation invocation)
        {
            var callStack = _callStack.Value;

            if (callStack == null || callStack.Count == 0)
            {
                throw new DiverterException("Fatal error: Encountered an unexpected Router proxy call end state");
            }

            var redirectRelay = callStack[callStack.Count - 1];
            
            if (!ReferenceEquals(invocation, redirectRelay.RootInvocation))
            {
                throw new DiverterException("Fatal error: Encountered an unexpected redirect relay for the current call");
            }
            
            callStack.RemoveAt(callStack.Count - 1);
        }

        public RedirectRelay<T> Current
        {
            get
            {
                var callStack = _callStack.Value;

                if (callStack == null || callStack.Count == 0)
                {
                    throw new DiverterException("Members of this instance may only be accessed from within the context of their Router proxy calls");
                }

                return callStack[callStack.Count - 1];
            }
        }
    }
}