using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Divertr.Internal
{
    internal class CallRelay<T> : ICallRelay<T> where T : class
    {
        private readonly AsyncLocal<List<RedirectContext<T>>> _callStack = new AsyncLocal<List<RedirectContext<T>>>();
        public T Next { get; }
        
        public T Original { get; }

        public object? State
        {
            get
            {
                var redirectContext = Peek();
                var redirect = redirectContext.Current;

                if (redirect == null)
                {
                    throw new DiverterException("Members of this instance may only be accessed from within the context of its DivertR proxy calls");
                }

                return redirect.State;
            }
        } 

        public CallRelay()
        {
            Next =  ProxyFactory.Instance.CreateRedirectTargetProxy(this);
            Original = ProxyFactory.Instance.CreateOriginalTargetProxy(this);
        }
        
        public void Push(RedirectContext<T> redirectContext)
        {
            var callStack = _callStack.Value?.ToList() ?? new List<RedirectContext<T>>();
            callStack.Add(redirectContext);
            _callStack.Value = callStack;
        }

        public RedirectContext<T>? Pop()
        {
            var callStack = _callStack.Value;

            if (callStack == null || callStack.Count == 0)
            {
                return null;
            }

            var invocationState = callStack[callStack.Count - 1];
            callStack.RemoveAt(callStack.Count - 1);

            return invocationState;
        }

        public RedirectContext<T> Peek()
        {
            var callStack = _callStack.Value;

            if (callStack == null || callStack.Count == 0)
            {
                throw new DiverterException("Members of this instance may only be accessed from within the context of its DivertR proxy calls");
            }

            return callStack[callStack.Count - 1];
        }
    }
}